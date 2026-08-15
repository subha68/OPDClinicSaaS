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

        public int Add(Clinic clinic, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Clinics (ClinicCode, Name, Address, Phone, LogoPath)
                                VALUES (@code, @name, @addr, @phone, @logoPath);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@code", clinic.ClinicCode);
            cmd.Parameters.AddWithValue("@name", clinic.Name);
            cmd.Parameters.AddWithValue("@addr", clinic.Address);
            cmd.Parameters.AddWithValue("@phone", clinic.Phone);
            cmd.Parameters.AddWithValue("@logoPath", clinic.LogoPath);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Clinic clinic, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Clinics
                             SET ClinicCode = @code,
                                 Name = @name,
                                 Address = @addr,
                                 Phone = @phone,
                                 LogoPath = @logoPath
                             WHERE ClinicId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@code", clinic.ClinicCode);
            cmd.Parameters.AddWithValue("@name", clinic.Name);
            cmd.Parameters.AddWithValue("@addr", clinic.Address);
            cmd.Parameters.AddWithValue("@phone", clinic.Phone);
            cmd.Parameters.AddWithValue("@logoPath", clinic.LogoPath);
            cmd.Parameters.AddWithValue("@id", clinic.ClinicId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Clinics WHERE ClinicId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Clinic> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Clinic>();

            var cmd = new SqlCommand("SELECT * FROM Clinics", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Clinic
                {
                    ClinicId = (int)reader["ClinicId"],
                    ClinicCode = reader["ClinicCode"].ToString(),
                    Name = reader["Name"].ToString(),
                    Address = reader["Address"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    LogoPath = reader["LogoPath"].ToString()
                });
            }

            return list;
        }

        public Clinic Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            Clinic clinic = null;

            var cmd = new SqlCommand("SELECT * FROM Clinics WHERE ClinicId = @id", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                clinic = new Clinic
                {
                    ClinicId = (int)reader["ClinicId"],
                    ClinicCode = reader["ClinicCode"].ToString(),
                    Name = reader["Name"].ToString(),
                    Address = reader["Address"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    LogoPath = reader["LogoPath"].ToString()
                };
            }

            return clinic;
        }

        public Clinic ValidateClinicCode(string clinicCode, SqlConnection conn, SqlTransaction? tran = null)
        {
            Clinic clinic = null;

            var cmd = new SqlCommand("SELECT * FROM Clinics WHERE ClinicCode = @code", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@code", clinicCode);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                clinic = new Clinic
                {
                    ClinicId = (int)reader["ClinicId"],
                    ClinicCode = reader["ClinicCode"].ToString(),
                    Name = reader["Name"].ToString(),
                    Address = reader["Address"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    LogoPath = reader["LogoPath"].ToString()
                };
            }

            return clinic;
        }
    }
}