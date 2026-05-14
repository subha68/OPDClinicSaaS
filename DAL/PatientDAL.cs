using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class PatientDAL
    {
        private readonly DbHelper _db;

        public PatientDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public Patient GetByMobile(string mobile)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT * FROM Patients WHERE Phone = @Phone";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Phone", mobile);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new Patient
                    {
                        PatientId = Convert.ToInt32(dr["PatientId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Phone = dr["Phone"].ToString()
                    };
                }
            }

            return null;
        }
    }
}

