using System.Data.SqlClient;
using OPDClinic.Web.Models;

namespace OPDClinic.Web.DAL
{
    public class UserDAL
    {
        private DbHelper db = new DbHelper();

        public User ValidateUser(string username, string password)
        {
            using (SqlConnection conn = db.GetConnection())
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
                        ClinicId = reader["ClinicId"] as int?,
                        RoleId = (int)reader["RoleId"],
                        Username = reader["Username"].ToString(),
                        IsFirstLogin = (bool)reader["IsFirstLogin"]
                    };
                }
            }

            return null;
        }
    }
}