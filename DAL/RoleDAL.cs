using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Models;

namespace DAL
{
    public class RoleDAL
    {
        private readonly DbHelper _db;

        public RoleDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public void Add(Role role)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO Roles (RoleName) VALUES (@RoleName)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Role> GetRoles()
        {
            var list = new List<Role>();

            using (SqlConnection conn = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Roles", conn);

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Role
                    {
                        RoleId = (int)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString()
                    });
                }
            }
            return list;
        }
    }
}