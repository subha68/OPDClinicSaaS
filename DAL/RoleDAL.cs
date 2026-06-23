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

        public List<Role> GetAll(SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Role>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Roles", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Role
                {
                    RoleId = (int)reader["RoleId"],
                    RoleName = reader["RoleName"].ToString()
                });
            }

            return list;
        }

        public Role GetRoleByName(string roleName, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = "SELECT * FROM Roles WHERE RoleName = @RoleName";
            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@RoleName", roleName);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Role
                {
                    RoleId = (int)reader["RoleId"],
                    RoleName = reader["RoleName"].ToString()
                };
            }

            return null; // Not found
        }

        public Role Get(int roleId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = "SELECT * FROM Roles WHERE RoleId = @RoleId";
            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@RoleId", roleId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Role
                {
                    RoleId = (int)reader["RoleId"],
                    RoleName = reader["RoleName"].ToString()
                };
            }

            return null; // Not found
        }
    }
}