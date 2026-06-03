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

        public int Add(Patient patient, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Patients (FirstName, LastName, Phone, DOB, Gender, Address, ClinicId)
                                VALUES (@first, @last, @phone, @dob, @gender, @address, @clinicId);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@first", string.IsNullOrWhiteSpace(patient.FirstName) ? DBNull.Value : (object)patient.FirstName);
            cmd.Parameters.AddWithValue("@last", string.IsNullOrWhiteSpace(patient.LastName) ? DBNull.Value : (object)patient.LastName);
            cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(patient.Phone) ? DBNull.Value : (object)patient.Phone);
            cmd.Parameters.AddWithValue("@dob", patient.DOB == DateTime.MinValue ? DBNull.Value : patient.DOB);
            cmd.Parameters.AddWithValue("@gender", string.IsNullOrWhiteSpace(patient.Gender) ? DBNull.Value : (object)patient.Gender);
            cmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(patient.Address) ? DBNull.Value : (object)patient.Address);

            cmd.Parameters.AddWithValue("@clinicId", patient.ClinicId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Patient patient, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Patients
                                SET FirstName = @first,
                                    LastName = @last,
                                    Phone = @phone,
                                    DOB = @dob,
                                    Gender = @gender,
                                    Address = @address
                                WHERE PatientId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@first", string.IsNullOrWhiteSpace(patient.FirstName) ? DBNull.Value : (object)patient.FirstName);
            cmd.Parameters.AddWithValue("@last", string.IsNullOrWhiteSpace(patient.LastName) ? DBNull.Value : (object)patient.LastName);
            cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(patient.Phone) ? DBNull.Value : (object)patient.Phone);
            cmd.Parameters.AddWithValue("@dob", patient.DOB == DateTime.MinValue ? DBNull.Value : patient.DOB);
            cmd.Parameters.AddWithValue("@gender", string.IsNullOrWhiteSpace(patient.Gender) ? DBNull.Value : (object)patient.Gender);
            cmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(patient.Address) ? DBNull.Value : (object)patient.Address);
            cmd.Parameters.AddWithValue("@id", patient.PatientId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Patients WHERE PatientId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
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
                        FirstName = dr["FirstName"]?.ToString() ?? "",
                        LastName = dr["LastName"]?.ToString() ?? "",
                        Phone = dr["Phone"]?.ToString() ?? "",
                        DOB = dr["DOB"] == DBNull.Value ? null : Convert.ToDateTime(dr["DOB"]),
                        Gender = dr["Gender"]?.ToString() ?? "",
                        Address = dr["Address"]?.ToString() ?? "",
                        ClinicId = Convert.ToInt32(dr["ClinicId"])
                    });
                }
            }

            return patients;
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
                        FirstName = dr["FirstName"]?.ToString() ?? "",
                        LastName = dr["LastName"]?.ToString() ?? "",
                        Phone = dr["Phone"]?.ToString() ?? "",
                        DOB = dr["DOB"] == DBNull.Value ? null : Convert.ToDateTime(dr["DOB"]),
                        Gender = dr["Gender"]?.ToString() ?? "",
                        Address = dr["Address"]?.ToString() ?? "",
                        ClinicId = Convert.ToInt32(dr["ClinicId"])
                    };
                }
            }

            return null;
        }

        public List<Patient> Search(PatientSearchVM vm)
        {
            var patients = new List<Patient>();

            SqlConnection con = _db.GetConnection();

            string sql = @"
                SELECT PatientId, FirstName, LastName, DOB, Gender, Address, Phone, ClinicId
                FROM Patients
                WHERE
                   (PatientId LIKE '%' + @searchString + '%')
                OR (Phone LIKE '%' + @searchString + '%')
                OR (FirstName + ' ' + LastName LIKE '%' + @searchString + '%')
                AND ClinicId = @ClinicId
                ORDER BY PatientId DESC";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@searchString", vm.SearchString);
            cmd.Parameters.AddWithValue("@ClinicId", vm.ClinicId);

            con.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                patients.Add(new Patient
                {
                    PatientId = Convert.ToInt32(reader["PatientId"]),
                    FirstName = reader["FirstName"]?.ToString() ?? "",
                    LastName = reader["LastName"]?.ToString() ?? "",
                    Phone = reader["Phone"]?.ToString() ?? "",
                    DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]),
                    Gender = reader["Gender"]?.ToString() ?? "",
                    Address = reader["Address"]?.ToString() ?? "",
                    ClinicId = Convert.ToInt32(reader["ClinicId"])
                });
            }

            return patients;
        }

        public List<Patient> ListByClinic(int clinicId)
        {
            var patients = new List<Patient>();

            SqlConnection con = _db.GetConnection();

            string sql = @"
                SELECT PatientId, FirstName, LastName, DOB, Gender, Address, Phone, ClinicId
                FROM Patients
                WHERE ClinicId = @ClinicId
                ORDER BY PatientId DESC";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            con.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                patients.Add(new Patient
                {
                    PatientId = Convert.ToInt32(reader["PatientId"]),
                    FirstName = reader["FirstName"]?.ToString() ?? "",
                    LastName = reader["LastName"]?.ToString() ?? "",
                    DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]),
                    Gender = reader["Gender"]?.ToString() ?? "",
                    Address = reader["Address"]?.ToString() ?? "",
                    Phone = reader["Phone"]?.ToString() ?? "",
                    ClinicId = Convert.ToInt32(reader["ClinicId"])
                });
            }

            return patients;
        }

    }
}

