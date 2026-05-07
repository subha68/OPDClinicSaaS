using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class DepartmentDAL
    {
        private readonly DbHelper _db;

        public DepartmentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public void AddDepartment(DepartmentVM model)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"INSERT INTO Departments (ClinicId, Name, Description)
                                VALUES (@ClinicId, @Name, @Description)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<DepartmentVM> GetDepartmentsByClinic(int clinicId)
        {
            List<DepartmentVM> list = new List<DepartmentVM>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = "SELECT * FROM Departments WHERE ClinicId=@ClinicId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

                con.Open();
                SqlDataReader dept = cmd.ExecuteReader();

                while (dept.Read())
                {
                    list.Add(new DepartmentVM
                    {
                        DepartmentId = Convert.ToInt32(dept["DepartmentId"]),
                        Name = dept["Name"].ToString(),
                        Description = dept["Description"].ToString()
                    });
                }
            }

            return list;
        }

    }
}