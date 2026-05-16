using Models;
using ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class ConsultationDAL
    {
        private readonly DbHelper _db;

        public ConsultationDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public void SaveConsultation(ConsultationVM vm)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    INSERT INTO Consultations
                    (
                        AppointmentId,
                        Symptoms,
                        Diagnosis,
                        Notes
                    )
                    VALUES
                    (
                        @AppointmentId,
                        @Symptoms,
                        @Diagnosis,
                        @Notes
                    );";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", vm.AppointmentId);
                    cmd.Parameters.AddWithValue("@Symptoms", vm.Symptoms ?? "");
                    cmd.Parameters.AddWithValue("@Diagnosis", vm.Diagnosis ?? "");
                    cmd.Parameters.AddWithValue("@Notes", vm.Notes ?? "");

                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

}

