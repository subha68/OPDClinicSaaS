using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using OPDClinic.Web.Models;

namespace OPDClinic.Web.DAL
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
    }
}