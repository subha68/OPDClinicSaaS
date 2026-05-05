using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class ClinicDAL
    {
        private readonly DbHelper _db;

        public ClinicDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public void AddClinic(Clinic clinic)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Clinics (ClinicCode, Name, Address, ContactNumber)
                                 VALUES (@code, @name, @addr, @contact)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@code", clinic.ClinicCode);
                cmd.Parameters.AddWithValue("@name", clinic.Name);
                cmd.Parameters.AddWithValue("@addr", clinic.Address);
                cmd.Parameters.AddWithValue("@contact", clinic.ContactNumber);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Clinic> GetClinics()
        {
            var list = new List<Clinic>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Clinics", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Clinic
                    {
                        ClinicId = (int)reader["ClinicId"],
                        ClinicCode = reader["ClinicCode"].ToString(),
                        Name = reader["Name"].ToString(),
                        Address = reader["Address"].ToString(),
                        ContactNumber = reader["ContactNumber"].ToString()
                    });
                }
            }

            return list;
        }
    }
}