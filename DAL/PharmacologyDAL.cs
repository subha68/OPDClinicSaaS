using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class PharmacologyDAL : IDAL<PharmacologyAdvice>
    {
        private readonly DbHelper _db;
        public int consultationId = 0;
        public int appointmentId = 0;

        public PharmacologyDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }
    
        public int Add(PharmacologyAdvice advice)
        {    
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    INSERT INTO PharmacologyAdvices
                    (
                        ConsultationId,
                        AppointmentId,
                        MedicineName,
                        Dosage,
                        Frequency,
                        Duration,
                        Instructions
                    )
                    VALUES
                    (
                        @ConsultationId,
                        @AppointmentId,
                        @MedicineName,
                        @Dosage,
                        @Frequency,
                        @Duration,
                        @Instructions
                    );
                    SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                cmd.Parameters.AddWithValue("@MedicineName", advice.MedicineName);
                cmd.Parameters.AddWithValue("@Dosage", advice.Dosage);
                cmd.Parameters.AddWithValue("@Frequency", advice.Frequency);
                cmd.Parameters.AddWithValue("@Duration", advice.Duration);
                cmd.Parameters.AddWithValue("@Instructions", advice.Instructions);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Update(PharmacologyAdvice model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    UPDATE PharmacologyAdvices
                    SET 
                        MedicineName = @MedicineName,
                        Dosage = @Dosage,
                        Frequency = @Frequency,
                        Duration = @Duration,
                        Instructions = @Instructions
                    WHERE ConsultationId = @ConsultationId AND AppointmentId = @AppointmentId;";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                cmd.Parameters.AddWithValue("@MedicineName", model.MedicineName);
                cmd.Parameters.AddWithValue("@Dosage", model.Dosage);
                cmd.Parameters.AddWithValue("@Frequency", model.Frequency);
                cmd.Parameters.AddWithValue("@Duration", model.Duration);
                cmd.Parameters.AddWithValue("@Instructions", model.Instructions);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    DELETE FROM PharmacologyAdvices
                    WHERE ConsultationId = @ConsultationId AND AppointmentId = @AppointmentId;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<PharmacologyAdvice> GetAll()
        {
            var list = new List<PharmacologyAdvice>();

            using (var conn = _db.GetConnection())
            {
                string query = @"
                    SELECT ph.*, a.PatientId
                    FROM PharmacologyAdvices ph
                    INNER JOIN Appointments a ON ph.AppointmentId = a.AppointmentId
                    WHERE ph.ConsultationId = @ConsultationId AND ph.AppointmentId = @AppointmentId;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var advice = new PharmacologyAdvice
                        {
                            PrescriptionId = Convert.ToInt32(reader["PrescriptionId"]),
                            ConsultationId = Convert.ToInt32(reader["ConsultationId"]),
                            AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                            PatientId = Convert.ToInt32(reader["PatientId"]),
                            MedicineName = reader["MedicineName"].ToString(),
                            Dosage = reader["Dosage"].ToString(),
                            Frequency = reader["Frequency"].ToString(),
                            Duration = reader["Duration"].ToString(),
                            Instructions = reader["Instructions"].ToString()
                        };
                        list.Add(advice);
                    }
                }
            }

            return list;
        }

        public PharmacologyAdvice Get(int id)
        {
            using (var conn = _db.GetConnection())
            {
                string query = @"
                    SELECT ph.*, a.PatientId
                    FROM PharmacologyAdvices ph
                    INNER JOIN Appointments a ON ph.AppointmentId = a.AppointmentId
                    WHERE ph.ConsultationId = @ConsultationId AND ph.AppointmentId = @AppointmentId;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PharmacologyAdvice
                        {
                            MedicineName = reader["MedicineName"].ToString(),
                            Dosage = reader["Dosage"].ToString(),
                            Frequency = reader["Frequency"].ToString(),
                            PatientId = Convert.ToInt32(reader["PatientId"]),
                            Duration = reader["Duration"].ToString(),
                            Instructions = reader["Instructions"].ToString()
                        };
                    }
                }
            }

            return null; // Not found
        }

        public List<PharmacologyAdvice> ListByPatient(int patientId)
        {
            var list = new List<PharmacologyAdvice>();

            using (var conn = _db.GetConnection())
            {
                string query = @"
                    SELECT ph.*, a.PatientId
                    FROM PharmacologyAdvices ph
                    INNER JOIN Appointments a ON ph.AppointmentId = a.AppointmentId
                    WHERE a.PatientId = @PatientId;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var advice = new PharmacologyAdvice
                        {
                            PrescriptionId = Convert.ToInt32(reader["PrescriptionId"]),
                            ConsultationId = Convert.ToInt32(reader["ConsultationId"]),
                            AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                            PatientId = Convert.ToInt32(reader["PatientId"]),
                            MedicineName = reader["MedicineName"].ToString(),
                            Dosage = reader["Dosage"].ToString(),
                            Frequency = reader["Frequency"].ToString(),
                            Duration = reader["Duration"].ToString(),
                            Instructions = reader["Instructions"].ToString()
                        };
                        list.Add(advice);
                    }
                }
            }

            return list;
        }
    }
}