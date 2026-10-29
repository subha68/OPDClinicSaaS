using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Models;

namespace DAL
{
    public class UserDAL : BaseDAL, IDAL<User>
    {
        private readonly DbHelper _db;

        public UserDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public User GetUserByName(string username, int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {           
            string query = @"SELECT  u.*
                            FROM Users u
                            INNER JOIN Roles r
                            ON u.RoleId = r.RoleId
                            WHERE u.Username = @Username
                            AND
                            (
                                r.RoleName = 'SuperAdmin'
                                OR
                                (
                                    r.RoleName IN ('ClinicAdmin','ClinicUser','Doctor','BillingUser','PharmacyUser', 'Nurse')
                                    AND (u.ClinicId IS NULL OR u.ClinicId = @ClinicId)
                                )
                            )
                            AND u.IsActive = 1";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId : 0);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    UserId = (int)reader["UserId"],
                    RoleId = (int)reader["RoleId"],
                    ClinicId = reader["ClinicId"] != DBNull.Value ? (int?)reader["ClinicId"] : null,
                    Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                    PasswordHash = reader["PasswordHash"] != DBNull.Value ? reader["PasswordHash"].ToString() : null,
                    FirstName = reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : null,
                    LastName = reader["LastName"] != DBNull.Value ? reader["LastName"].ToString() : null,
                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                    Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null,
                    IsFirstLogin = Convert.ToBoolean(reader["IsFirstLogin"])
                };
            }

            return null;
        }

        public int Add(User user, SqlConnection conn, SqlTransaction tran)
        {
            int userId = 0;

            string query = @"INSERT INTO Users 
                            (Username, PasswordHash, RoleId, ClinicId, FirstName, LastName, Email, Phone, IsFirstLogin)
                            VALUES (@Username, @PasswordHash, @RoleId, @ClinicId, @FirstName, @LastName, @Email, @Phone, 1);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
            cmd.Parameters.AddWithValue("@ClinicId", user.ClinicId);
            cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
            cmd.Parameters.AddWithValue("@LastName", user.LastName);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Phone", user.Phone);

            userId = Convert.ToInt32(cmd.ExecuteScalar());

            return userId;
        }

        public bool Update(User user, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Users
                            SET Username = @Username,
                                PasswordHash = @PasswordHash,
                                RoleId = @RoleId,
                                ClinicId = @ClinicId,
                                FirstName = @FirstName,
                                LastName = @LastName,
                                Email = @Email,
                                Phone = @Phone,
                                IsFirstLogin = @IsFirstLogin
                            WHERE UserId = @UserId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
            cmd.Parameters.AddWithValue("@ClinicId", user.ClinicId.HasValue ? (object)user.ClinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@FirstName", user.FirstName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName", user.LastName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UserId", user.UserId);
            cmd.Parameters.AddWithValue("@IsFirstLogin", user.IsFirstLogin ? 1 : 0);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int userId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Users WHERE UserId = @UserId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@UserId", userId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public User Get(int userId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT u.*, c.Name as ClinicName, r.RoleName 
                                FROM Users u
                                LEFT JOIN Clinics c ON u.ClinicId = c.ClinicId
                                LEFT JOIN Roles r ON u.RoleId = r.RoleId
                                WHERE u.UserId = @UserId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    UserId = (int)reader["UserId"],
                    RoleId = (int)reader["RoleId"],
                    ClinicId = reader["ClinicId"] != DBNull.Value ? (int?)reader["ClinicId"] : null,
                    Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : null,
                    FirstName = reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : null,
                    LastName = reader["LastName"] != DBNull.Value ? reader["LastName"].ToString() : null,
                    PasswordHash = reader["PasswordHash"] != DBNull.Value ? reader["PasswordHash"].ToString() : null,
                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                    Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null,
                    ClinicName = reader["ClinicName"] != DBNull.Value ? reader["ClinicName"].ToString() : null,
                    RoleName = reader["RoleName"] != DBNull.Value ? reader["RoleName"].ToString() : null,
                    IsFirstLogin = Convert.ToBoolean(reader["IsFirstLogin"])
                };
            }

            return null;
        }

        public int GetUserIdByName(string username, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT UserId 
                                FROM Users
                                WHERE Username = @username";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@username", username);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return (int)reader["UserId"];
            }

            return 0;
        }

        public int GetUserCountByName(string username, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = "SELECT COUNT(*) AS UserCount FROM Users WHERE Username=@Username";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@Username", username);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count;
        }

        public List<User> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<User>();

            SqlCommand cmd = new SqlCommand(@"SELECT u.*, c.Name as ClinicName, r.RoleName 
                                            FROM Users u 
                                            LEFT JOIN Clinics c ON u.ClinicId = c.ClinicId 
                                            LEFT JOIN Roles r ON u.RoleId = r.RoleId", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new User
                {
                    UserId = (int)reader["UserId"],
                    Username = reader["Username"].ToString(),
                    RoleId = (int)reader["RoleId"],
                    ClinicId = reader["ClinicId"] != DBNull.Value ? (int?)reader["ClinicId"] : null,
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Email = reader["Email"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    ClinicName = reader["ClinicName"] != DBNull.Value ? reader["ClinicName"].ToString() : null,
                    RoleName = reader["RoleName"] != DBNull.Value ? reader["RoleName"].ToString() : null
                });
            }

            return list;
        }

        public List<User> ListByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<User>();

            SqlCommand cmd = new SqlCommand(@"SELECT u.*, c.Name as ClinicName, r.RoleName 
                                            FROM Users u 
                                            LEFT JOIN Clinics c ON u.ClinicId = c.ClinicId 
                                            LEFT JOIN Roles r ON u.RoleId = r.RoleId 
                                            WHERE u.ClinicId = @ClinicId", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new User
                {
                    UserId = (int)reader["UserId"],
                    Username = reader["Username"].ToString(),
                    RoleId = (int)reader["RoleId"],
                    ClinicId = (int?)reader["ClinicId"],
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Email = reader["Email"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    ClinicName = reader["ClinicName"] != DBNull.Value ? reader["ClinicName"].ToString() : null,
                    RoleName = reader["RoleName"] != DBNull.Value ? reader["RoleName"].ToString() : null
                });
            }

            return list;
        }

        public List<Role> GetRoles(SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Role>();

            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Roles", conn);
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
    }
}