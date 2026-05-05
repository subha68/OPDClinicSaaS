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
                        RoleId = (int)reader["RoleId"]
                    };
                }
            }

            return null;
        }

        public void Add(User user)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO Users 
                                (Username, PasswordHash, RoleId, ClinicId, FirstName, LastName, Email, ContactNumber)
                                VALUES (@Username, @PasswordHash, @RoleId, @ClinicId, @FirstName, @LastName, @Email, @ContactNumber)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                cmd.Parameters.AddWithValue("@ClinicId", user.ClinicId);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@ContactNumber", user.ContactNumber);

                conn.Open();
                cmd.ExecuteNonQuery();
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
                        ClinicId = (int)reader["ClinicId"],
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        ContactNumber = reader["ContactNumber"].ToString()
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