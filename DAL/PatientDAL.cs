using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class PatientDAL : IDAL<Patient>
    {
        private readonly DbHelper _db;

        public PatientDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Patient patient)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Patients (FirstName, LastName, Phone)
                                 VALUES (@first, @last, @phone);
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@first", patient.FirstName);
                cmd.Parameters.AddWithValue("@last", patient.LastName);
                cmd.Parameters.AddWithValue("@phone", patient.Phone);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Update(Patient patient)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"UPDATE Patients
                                 SET FirstName = @first,
                                     LastName = @last,
                                     Phone = @phone
                                 WHERE PatientId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@first", patient.FirstName);
                cmd.Parameters.AddWithValue("@last", patient.LastName);
                cmd.Parameters.AddWithValue("@phone", patient.Phone);
                cmd.Parameters.AddWithValue("@id", patient.PatientId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"DELETE FROM Patients WHERE PatientId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<Patient> GetAll()
        {
            List<Patient> patients = new List<Patient>();

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT * FROM Patients";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    patients.Add(new Patient
                    {
                        PatientId = Convert.ToInt32(dr["PatientId"]),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Phone = dr["Phone"].ToString()
                    });
                }
            }

            return patients;
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

        public Patient Get(int id)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT * FROM Patients WHERE PatientId = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);

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

