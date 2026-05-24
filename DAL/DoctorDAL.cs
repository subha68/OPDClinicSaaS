using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class DoctorDAL : IDAL<Doctor>
    {
        private readonly DbHelper _db;

        public DoctorDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Doctor model)
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

        public bool Update(Doctor model)
        {
            int rowsAffected_doctor = 0;

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"UPDATE Doctors
                                SET DepartmentId = @DepartmentId,
                                    FirstName = @FirstName,
                                    LastName = @LastName,
                                    RegistrationNo = @RegNo,
                                    Qualification = @Qualification
                                WHERE DoctorId = @DoctorId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
                cmd.Parameters.AddWithValue("@RegNo", model.RegistrationNo ?? "");
                cmd.Parameters.AddWithValue("@Qualification", model.Qualification ?? "");
                cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);

                con.Open();
                rowsAffected_doctor = cmd.ExecuteNonQuery();
            }

            // Also update the associated User record's name fields
            int rowsAffected_user   = 0;

            using (SqlConnection con = _db.GetConnection())
            {   
                string query = @"UPDATE Users
                                SET FirstName = @FirstName,
                                    LastName = @LastName,
                                    Username = @Username,
                                    PasswordHash = @PasswordHash
                                WHERE UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
                cmd.Parameters.AddWithValue("@Username", model.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", model.Password); // In real app, hash the password
                cmd.Parameters.AddWithValue("@UserId", model.UserId);

                con.Open();
                rowsAffected_user = cmd.ExecuteNonQuery();
            }

            return rowsAffected_user > 0 && rowsAffected_doctor > 0;
        }

        public bool Delete(int id)
        {
            int rowsAffected_doctor = 0;
            int userId = Get(id).UserId;

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"DELETE FROM Doctors WHERE DoctorId = @DoctorId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorId", id);

                con.Open();
                rowsAffected_doctor = cmd.ExecuteNonQuery();
            }

            int rowsAffected_user = 0;

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"DELETE FROM Users WHERE UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", id);

                con.Open();
                rowsAffected_user = cmd.ExecuteNonQuery();
            }
            

            return rowsAffected_doctor > 0 && rowsAffected_user > 0;
        }

        public List<Doctor> GetAll()
        {
            List<Doctor> list = new List<Doctor>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT d.*, dept.Name AS DepartmentName, c.Name AS ClinicName
                                FROM Doctors d
                                INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                                INNER JOIN Clinics c ON d.ClinicId = c.ClinicId";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Doctor
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        ClinicId = Convert.ToInt32(dr["ClinicId"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        RegistrationNo = dr["RegistrationNo"].ToString(),
                        Qualification = dr["Qualification"].ToString(),
                        DepartmentName = dr["DepartmentName"].ToString(),
                        ClinicName = dr["ClinicName"].ToString()
                    });
                }
            }

            return list;
        }

        public Doctor Get(int doctorId)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT d.*, dept.Name AS DepartmentName, c.Name AS ClinicName
                                FROM Doctors d
                                INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                                INNER JOIN Clinics c ON d.ClinicId = c.ClinicId
                                WHERE d.DoctorId = @DoctorId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new Doctor
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        ClinicId = Convert.ToInt32(dr["ClinicId"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        RegistrationNo = dr["RegistrationNo"].ToString(),
                        Qualification = dr["Qualification"].ToString(),
                        DepartmentName = dr["DepartmentName"].ToString(),
                        ClinicName = dr["ClinicName"].ToString()
                    };
                }
            }

            return null; // Not found
        }

        public List<Doctor> GetDoctorsByClinic(int clinicId)
        {
            List<Doctor> list = new List<Doctor>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT d.*, dept.Name AS DepartmentName, c.Name AS ClinicName
                                FROM Doctors d
                                INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                                INNER JOIN Clinics c ON d.ClinicId = c.ClinicId
                                WHERE d.ClinicId = @ClinicId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Doctor
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        ClinicId = Convert.ToInt32(dr["ClinicId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        DepartmentName = dr["DepartmentName"].ToString(),
                        ClinicName = dr["ClinicName"].ToString()
                    });
                }
            }

            return list;
        }

        public List<Doctor> GetDoctorsByDepartment(int clinicId, int departmentId)
        {
            List<Doctor> list = new List<Doctor>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT d.*, dept.Name AS DepartmentName, c.Name AS ClinicName
                                FROM Doctors d
                                INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                                INNER JOIN Clinics c ON d.ClinicId = c.ClinicId
                                WHERE d.ClinicId = @ClinicId AND d.DepartmentId = @DepartmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Doctor
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        DepartmentName = dr["DepartmentName"].ToString(),
                        ClinicName = dr["ClinicName"].ToString()
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