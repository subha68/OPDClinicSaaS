using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class DoctorDAL : IDAL<DoctorVM>
    {
        private readonly DbHelper _db;

        public DoctorDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(DoctorVM model)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"INSERT INTO Doctors 
                                (ClinicId, DepartmentId, FirstName, LastName, RegistrationNo, Qualification)
                                VALUES
                                (@ClinicId, @DepartmentId, @FirstName, @LastName, @RegNo, @Qualification);
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
                cmd.Parameters.AddWithValue("@RegNo", model.RegistrationNo ?? "");
                cmd.Parameters.AddWithValue("@Qualification", model.Qualification ?? "");
        
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Update(DoctorVM model)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"UPDATE Doctors
                                SET ClinicId = @ClinicId,
                                    DepartmentId = @DepartmentId,
                                    FirstName = @FirstName,
                                    LastName = @LastName,
                                    RegistrationNo = @RegNo,
                                    Qualification = @Qualification
                                WHERE DoctorId = @DoctorId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
                cmd.Parameters.AddWithValue("@RegNo", model.RegistrationNo ?? "");
                cmd.Parameters.AddWithValue("@Qualification", model.Qualification ?? "");
                cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"DELETE FROM Doctors WHERE DoctorId = @DoctorId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorId", id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<DoctorVM> GetAll()
        {
            List<DoctorVM> list = new List<DoctorVM>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT d.*, dept.Name AS DepartmentName
                                FROM Doctors d
                                INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new DoctorVM
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        ClinicId = Convert.ToInt32(dr["ClinicId"]),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        RegistrationNo = dr["RegistrationNo"].ToString(),
                        Qualification = dr["Qualification"].ToString(),
                        DepartmentName = dr["DepartmentName"].ToString()
                    });
                }
            }

            return list;
        }

        public DoctorVM Get(int doctorId)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT d.*, dept.Name AS DepartmentName
                                FROM Doctors d
                                INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                                WHERE d.DoctorId = @DoctorId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new DoctorVM
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        ClinicId = Convert.ToInt32(dr["ClinicId"]),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        RegistrationNo = dr["RegistrationNo"].ToString(),
                        Qualification = dr["Qualification"].ToString(),
                        DepartmentName = dr["DepartmentName"].ToString()
                    };
                }
            }

            return null; // Not found
        }

        public List<DoctorVM> GetDoctorsByClinic(int clinicId)
        {
            List<DoctorVM> list = new List<DoctorVM>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT d.*, dept.Name AS DepartmentName
                                FROM Doctors d
                                INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                                WHERE d.ClinicId = @ClinicId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new DoctorVM
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"])
                    });
                }
            }

            return list;
        }

        public List<DoctorVM> GetDoctorsByDepartment(int clinicId, int departmentId)
        {
            List<DoctorVM> list = new List<DoctorVM>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT d.*, dept.Name AS DepartmentName
                                FROM Doctors d
                                INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                                WHERE d.ClinicId = @ClinicId AND d.DepartmentId = @DepartmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new DoctorVM
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"])
                    });
                }
            }

            return list;
        }

        public int GetDoctorIdByUserId(int userId)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = "SELECT DoctorId FROM Doctors WHERE UserId=@UserId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();

                object result = cmd.ExecuteScalar();

                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

    }
}