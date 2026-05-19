using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class DepartmentDAL : IDAL<DepartmentVM>
    {
        private readonly DbHelper _db;

        public DepartmentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(DepartmentVM model)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"INSERT INTO Departments (ClinicId, Name, Description)
                                VALUES (@ClinicId, @Name, @Description);
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Update(DepartmentVM model)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"UPDATE Departments
                                 SET Name = @Name,
                                     Description = @Description
                                 WHERE DepartmentId = @DepartmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = "DELETE FROM Departments WHERE DepartmentId = @DepartmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DepartmentId", id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<DepartmentVM> GetAll()
        {
            List<DepartmentVM> list = new List<DepartmentVM>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = "SELECT * FROM Departments";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader dept = cmd.ExecuteReader();

                while (dept.Read())
                {
                    list.Add(new DepartmentVM
                    {
                        DepartmentId = Convert.ToInt32(dept["DepartmentId"]),
                        ClinicId = Convert.ToInt32(dept["ClinicId"]),
                        Name = dept["Name"].ToString(),
                        Description = dept["Description"].ToString()
                    });
                }
            }

            return list;
        }

        public DepartmentVM Get(int id)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = "SELECT * FROM Departments WHERE DepartmentId = @DepartmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DepartmentId", id);

                con.Open();
                SqlDataReader dept = cmd.ExecuteReader();

                if (dept.Read())
                {
                    return new DepartmentVM
                    {
                        DepartmentId = Convert.ToInt32(dept["DepartmentId"]),
                        ClinicId = Convert.ToInt32(dept["ClinicId"]),
                        Name = dept["Name"].ToString(),
                        Description = dept["Description"].ToString()
                    };
                }
            }

            return null; // Not found
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