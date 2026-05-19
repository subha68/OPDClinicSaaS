using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class ClinicDAL : IDAL<Clinic>
    {
        private readonly DbHelper _db;

        public ClinicDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Clinic clinic)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Clinics (ClinicCode, Name, Address, Phone)
                                 VALUES (@code, @name, @addr, @phone);
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@code", clinic.ClinicCode);
                cmd.Parameters.AddWithValue("@name", clinic.Name);
                cmd.Parameters.AddWithValue("@addr", clinic.Address);
                cmd.Parameters.AddWithValue("@phone", clinic.Phone);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Update(Clinic clinic)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"UPDATE Clinics
                                 SET ClinicCode = @code,
                                     Name = @name,
                                     Address = @addr,
                                     Phone = @phone
                                 WHERE ClinicId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@code", clinic.ClinicCode);
                cmd.Parameters.AddWithValue("@name", clinic.Name);
                cmd.Parameters.AddWithValue("@addr", clinic.Address);
                cmd.Parameters.AddWithValue("@phone", clinic.Phone);
                cmd.Parameters.AddWithValue("@id", clinic.ClinicId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int clinicId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"DELETE FROM Clinics WHERE ClinicId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", clinicId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<Clinic> GetAll()
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
                        Phone = reader["Phone"].ToString()
                    });
                }
            }

            return list;
        }

        public Clinic Get(int clinicId)
        {
            Clinic clinic = null;

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Clinics WHERE ClinicId = @id", conn);
                cmd.Parameters.AddWithValue("@id", clinicId);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    clinic = new Clinic
                    {
                        ClinicId = (int)reader["ClinicId"],
                        ClinicCode = reader["ClinicCode"].ToString(),
                        Name = reader["Name"].ToString(),
                        Address = reader["Address"].ToString(),
                        Phone = reader["Phone"].ToString()
                    };
                }
            }

            return clinic;
        }

        public Clinic ValidateClinicCode(string clinicCode)
        {
            Clinic clinic = null;

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Clinics WHERE ClinicCode = @code", conn);
                cmd.Parameters.AddWithValue("@code", clinicCode);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    clinic = new Clinic
                    {
                        ClinicId = (int)reader["ClinicId"],
                        ClinicCode = reader["ClinicCode"].ToString(),
                        Name = reader["Name"].ToString(),
                        Address = reader["Address"].ToString(),
                        Phone = reader["Phone"].ToString()
                    };
                }
            }

            return clinic;
        }
    }
}