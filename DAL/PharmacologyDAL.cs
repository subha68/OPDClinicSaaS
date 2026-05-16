using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class PharmacologyDAL
    {
        private readonly DbHelper _db;

        public PharmacologyDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }
    
        public void Add(int appointmentId, PharmacologyAdviceVM advice)
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
                    );";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ConsultationId", advice.ConsultationId);
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    cmd.Parameters.AddWithValue("@MedicineName", advice.MedicineName);
                    cmd.Parameters.AddWithValue("@Dosage", advice.Dosage);
                    cmd.Parameters.AddWithValue("@Frequency", advice.Frequency);
                    cmd.Parameters.AddWithValue("@Duration", advice.Duration);
                    cmd.Parameters.AddWithValue("@Instructions", advice.Instructions);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}