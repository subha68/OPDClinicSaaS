using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class DoctorDAL
    {
        private readonly DbHelper _db;

        public DoctorDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public void AddDoctor(DoctorVM model)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"INSERT INTO Doctors 
                                (ClinicId, DepartmentId, FirstName, LastName, RegistrationNo, Qualification)
                                VALUES
                                (@ClinicId, @DepartmentId, @FirstName, @LastName, @RegNo, @Qualification)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
                cmd.Parameters.AddWithValue("@RegNo", model.RegistrationNo ?? "");
                cmd.Parameters.AddWithValue("@Qualification", model.Qualification ?? "");
        
                con.Open();
                cmd.ExecuteNonQuery();
            }
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

    }
}