using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Models;

namespace DAL
{
    public class RoleDAL : IDAL<Role>
    {
        private readonly DbHelper _db;

        public RoleDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public int Add(Role role, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Roles (RoleName) VALUES (@RoleName);
                                SELECT SCOPE_IDENTITY();";
            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Role role, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Roles SET RoleName = @RoleName WHERE RoleId = @RoleId";
            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
            cmd.Parameters.AddWithValue("@RoleId", role.RoleId);
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Roles WHERE RoleId = @RoleId";
            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@RoleId", id);
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Role> GetAll()
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

        public Role GetRoleByName(string roleName)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT * FROM Roles WHERE RoleName = @RoleName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoleName", roleName);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Role
                    {
                        RoleId = (int)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString()
                    };
                }
            }
            return null; // Not found
        }

        public Role Get(int roleId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT * FROM Roles WHERE RoleId = @RoleId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoleId", roleId);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Role
                    {
                        RoleId = (int)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString()
                    };
                }
            }
            return null; // Not found
        }
    }
}