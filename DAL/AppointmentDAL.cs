using Models;
using ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class AppointmentDAL : IDAL<AppointmentVM>
    {
        private readonly DbHelper _db;

        public AppointmentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(AppointmentVM model)
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

        public bool Update(AppointmentVM model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    UPDATE Appointments
                    SET DoctorId = @DoctorId,
                        ClinicId = @ClinicId,
                        PatientId = @PatientId,
                        AppointmentDate = @AppointmentDate,
                        SlotTime = @SlotTime,
                        Status = @Status
                    WHERE AppointmentId = @AppointmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);
                    cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                    cmd.Parameters.AddWithValue("@PatientId", model.PatientId);
                    cmd.Parameters.AddWithValue("@AppointmentDate", model.AppointmentDate);
                    cmd.Parameters.AddWithValue("@SlotTime", model.SlotTime);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@AppointmentId", model.AppointmentId);

                    conn.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM Appointments WHERE AppointmentId = @AppointmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", id);

                    conn.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public List<AppointmentVM> GetAll()
        {
            var list = new List<AppointmentVM>();

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
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
                        ON a.DoctorId = d.DoctorId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
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
                    }
                }
            }

            return list;
        }

        public AppointmentVM Get(int appointmentId)
        {
            AppointmentVM appointment = null;

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
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
                    WHERE a.AppointmentId = @AppointmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            appointment = new AppointmentVM
                            {
                                AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                                AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                                SlotTime = (TimeSpan)dr["SlotTime"],
                                Status = dr["Status"].ToString(),
                                PatientName = dr["PatientName"].ToString(),
                                DoctorName = dr["DoctorName"].ToString()
                            };
                        }
                    }
                }
            }

            return appointment;
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
        
    }
}

