using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Models;

namespace DAL
{
    public class UserDAL
    {
        private readonly DbHelper _db;

        public UserDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }
        public User ValidateUser(string username, string password)
        {           
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = "SELECT * FROM Users WHERE Username=@u AND PasswordHash=@p";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

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

        public List<User> GetUsers()
        {
            var list = new List<User>();

            using (SqlConnection conn = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Users", conn);

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
                        Phone = reader["Phone"].ToString()
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