using Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class AppointmentDAL
    {
        private readonly DbHelper _db;

        public AppointmentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Insert(Appointment model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    INSERT INTO Appointments
                    (
                        DoctorId,
                        ClinicId,
                        PatientId,
                        AppointmentDate,
                        SlotTime,
                        Status
                    )
                    VALUES
                    (
                        @DoctorId,
                        @ClinicId,
                        @PatientId,
                        @AppointmentDate,
                        @SlotTime,
                        @Status
                    );

                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);
                    cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                    cmd.Parameters.AddWithValue("@PatientId", model.PatientId);
                    cmd.Parameters.AddWithValue("@AppointmentDate", model.AppointmentDate);
                    cmd.Parameters.AddWithValue("@SlotTime", model.SlotTime);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    conn.Open();

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

    }

}

