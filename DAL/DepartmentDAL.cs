using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class DepartmentDAL : IDAL<Department>
    {
        private readonly DbHelper _db;

        public DepartmentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Department model)
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

        public bool Update(Department model)
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

        public List<Department> GetAll()
        {
            List<Department> list = new List<Department>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = "SELECT d.*, c.Name as ClinicName FROM Departments d, Clinics c WHERE d.ClinicId = c.ClinicId";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader dept = cmd.ExecuteReader();

                while (dept.Read())
                {
                    list.Add(new Department
                    {
                        DepartmentId = Convert.ToInt32(dept["DepartmentId"]),
                        ClinicId = Convert.ToInt32(dept["ClinicId"]),
                        Name = dept["Name"].ToString(),
                        Description = dept["Description"].ToString(),
                        ClinicName = dept["ClinicName"].ToString()
                    });
                }
            }

            return list;
        }

        public Department Get(int id)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = "SELECT d.*, c.Name as ClinicName FROM Departments d, Clinics c WHERE d.ClinicId = c.ClinicId AND d.DepartmentId = @DepartmentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DepartmentId", id);

                con.Open();
                SqlDataReader dept = cmd.ExecuteReader();

                if (dept.Read())
                {
                    return new Department
                    {
                        DepartmentId = Convert.ToInt32(dept["DepartmentId"]),
                        ClinicId = Convert.ToInt32(dept["ClinicId"]),
                        Name = dept["Name"].ToString(),
                        Description = dept["Description"].ToString(),
                        ClinicName = dept["ClinicName"].ToString()
                    };
                }
            }

            return null; // Not found
        }

        public List<Department> GetDepartmentsByClinic(int clinicId)
        {
            List<Department> list = new List<Department>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = "SELECT d.*, c.Name as ClinicName FROM Departments d, Clinics c WHERE d.ClinicId = c.ClinicId AND d.ClinicId=@ClinicId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

                con.Open();
                SqlDataReader dept = cmd.ExecuteReader();

                while (dept.Read())
                {
                    list.Add(new Department
                    {
                        DepartmentId = Convert.ToInt32(dept["DepartmentId"]),
                        ClinicId = Convert.ToInt32(dept["ClinicId"]),
                        Name = dept["Name"].ToString(),
                        Description = dept["Description"].ToString(),
                        ClinicName = dept["ClinicName"].ToString()
                    });
                }
            }

            return list;
        }

    }
}