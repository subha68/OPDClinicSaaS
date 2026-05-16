using Models;
using ViewModels;
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

        public int Insert(AppointmentVM model)
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

        public List<AppointmentVM> GetTodaysAppointments(int doctorId)
        {
            var list = new List<AppointmentVM>();

            using var con = _db.GetConnection();

            string sql = @"
                SELECT
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.SlotTime,
                    a.Status,
                    p.FirstName + ' ' + p.LastName as PatientName,
                    d.FirstName + ' ' + d.LastName as DoctorName
                FROM Appointments a
                INNER JOIN Patients p
                    ON a.PatientId = p.PatientId
                INNER JOIN Doctors d
                    ON a.DoctorId = d.DoctorId
                WHERE a.DoctorId = @DoctorId
                    AND CAST(a.AppointmentDate as date)=CAST(GETDATE() as date)
                ORDER BY a.SlotTime";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@DoctorId", doctorId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new AppointmentVM
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                    SlotTime = (TimeSpan)dr["SlotTime"],
                    Status = dr["Status"].ToString(),
                    PatientName = dr["PatientName"].ToString(),
                    DoctorName = dr["DoctorName"].ToString()
                });
            }

            return list;
        }

        public void UpdateStatus(int appointmentId, string status)
        {
            using var con = _db.GetConnection();

            string sql = "UPDATE Appointments SET Status=@Status WHERE AppointmentId=@AppointmentId";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            con.Open();

            cmd.ExecuteNonQuery();
        }

        public ConsultationVM GetConsultationVM(int appointmentId)
        {
            ConsultationVM vm = null;

            using var con = _db.GetConnection();

            string sql = @"
                SELECT
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.SlotTime,
                    p.PatientId,
                    p.FirstName + ' ' + p.LastName as PatientName,
                    DATEDIFF(YEAR, p.DOB, GETDATE())
                        -
                        CASE
                            WHEN DATEADD(YEAR,
                                        DATEDIFF(YEAR, p.DOB, GETDATE()),
                                        p.DOB) > GETDATE()
                            THEN 1
                            ELSE 0
                        END AS Age,
                    d.DoctorId,
                    d.FirstName + ' ' + d.LastName as DoctorName
                FROM Appointments a
                INNER JOIN Patients p
                    ON a.PatientId = p.PatientId
                INNER JOIN Doctors d
                    ON a.DoctorId = d.DoctorId
                WHERE a.AppointmentId = @AppointmentId";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                vm = new ConsultationVM
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    //AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                    //SlotTime = Convert.ToDateTime(dr["SlotTime"]),
                    //PatientId = Convert.ToInt32(dr["PatientId"]),
                    PatientName = dr["PatientName"].ToString(),
                    //Age = Convert.ToInt32(dr["Age"]),
                    //DoctorId = Convert.ToInt32(dr["DoctorId"]),
                    //DoctorName = dr["DoctorName"].ToString()
                };
            }

            return vm;
        }

        
    }
}

