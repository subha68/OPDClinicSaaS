using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Models;

namespace DAL
{
    public class UserDAL : IDAL<User>
    {
        private readonly DbHelper _db;

        public UserDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }
        public User ValidateUser(string username, string password, int? clinicId)
        {           
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT  u.*
                                FROM Users u
                                INNER JOIN Roles r
                                ON u.RoleId = r.RoleId
                                WHERE u.Username = @Username
                                AND u.PasswordHash = @Password
                                AND
                                (
                                    r.RoleName = 'SuperAdmin'
                                    OR
                                    (
                                        r.RoleName IN ('ClinicAdmin','ClinicUser','Doctor','BillingUser')
                                        AND u.ClinicId = @ClinicId
                                    )
                                )
                                AND u.IsActive = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId : 0);

                SqlDataReader reader = cmd.ExecuteReader();

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
                        Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                        Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null
                    };
                }
            }

            return null;
        }

        public int Add(User user)
        {
            int userId = 0;

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO Users 
                                (Username, PasswordHash, RoleId, ClinicId, FirstName, LastName, Email, Phone)
                                VALUES (@Username, @PasswordHash, @RoleId, @ClinicId, @FirstName, @LastName, @Email, @Phone);
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                cmd.Parameters.AddWithValue("@ClinicId", user.ClinicId);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Phone", user.Phone);

                conn.Open();
                //cmd.ExecuteNonQuery();
                userId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return userId;
        }

        public bool Update(User user)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"UPDATE Users
                                SET Username = @Username,
                                    PasswordHash = @PasswordHash,
                                    RoleId = @RoleId,
                                    ClinicId = @ClinicId,
                                    FirstName = @FirstName,
                                    LastName = @LastName,
                                    Email = @Email,
                                    Phone = @Phone
                                WHERE UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                cmd.Parameters.AddWithValue("@ClinicId", user.ClinicId.HasValue ? (object)user.ClinicId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastName", user.LastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int userId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"DELETE FROM Users WHERE UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public User Get(int userId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT u.*, c.Name as ClinicName, r.RoleName 
                                  FROM Users u
                                  LEFT JOIN Clinics c ON u.ClinicId = c.ClinicId
                                  LEFT JOIN Roles r ON u.RoleId = r.RoleId
                                  WHERE u.UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                var reader = cmd.ExecuteReader();

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
                        Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                        Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null,
                        ClinicName = reader["ClinicName"] != DBNull.Value ? reader["ClinicName"].ToString() : null,
                        RoleName = reader["RoleName"] != DBNull.Value ? reader["RoleName"].ToString() : null
                    };
                }
            }
            return null;
        }
        public int GetUserCountByName(string username)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM [User] WHERE Username=@Username";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Username", username);

                con.Open();

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count;
            }
        }

        public List<User> GetAll()
        {
            var list = new List<User>();

            using (SqlConnection conn = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"SELECT u.*, c.ClinicName, r.RoleName 
                                                FROM Users u 
                                                LEFT JOIN Clinics c ON u.ClinicId = c.ClinicId 
                                                LEFT JOIN Roles r ON u.RoleId = r.RoleId", conn);

                conn.Open();
                var reader = cmd.ExecuteReader();

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
            }
            return list;
        }

        public List<User> GetAllByClinic(int clinicId)
        {
            var list = new List<User>();

            using (SqlConnection conn = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"SELECT u.*, c.Name as ClinicName, r.RoleName 
                                                FROM Users u 
                                                LEFT JOIN Clinics c ON u.ClinicId = c.ClinicId 
                                                LEFT JOIN Roles r ON u.RoleId = r.RoleId 
                                                WHERE u.ClinicId = @ClinicId", conn);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

                conn.Open();
                var reader = cmd.ExecuteReader();

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
            }
            return list;
        }

        public List<Role> GetRoles()
        {
            var list = new List<Role>();

            using (SqlConnection conn = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"SELECT * FROM Roles", conn);

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