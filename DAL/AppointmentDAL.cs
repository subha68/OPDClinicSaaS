using Models;
using ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class AppointmentDAL : IDAL<Appointment>
    {
        private readonly DbHelper _db;

        public AppointmentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Appointment model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    INSERT INTO Appointments
                    (
                        DoctorId,
                        ClinicId,
                        PatientId,
                        VisitId,
                        AppointmentDate,
                        SlotTime,
                        Status
                    )
                    VALUES
                    (
                        @DoctorId,
                        @ClinicId,
                        @PatientId,
                        @VisitId,
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
                    cmd.Parameters.AddWithValue("@VisitId", model.VisitId);
                    cmd.Parameters.AddWithValue("@AppointmentDate", model.AppointmentDate);
                    cmd.Parameters.AddWithValue("@SlotTime", model.SlotTime);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    conn.Open();

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool Update(Appointment model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    UPDATE Appointments
                    SET DoctorId = @DoctorId,
                        PatientId = @PatientId,
                        AppointmentDate = @AppointmentDate,
                        SlotTime = @SlotTime,
                        Status = @Status
                    WHERE AppointmentId = @AppointmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);
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

        public List<Appointment> GetAll()
        {
            var list = new List<Appointment>();

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        a.AppointmentId,
                        a.AppointmentDate,
                        v.VisitId,
                        a.SlotTime,
                        a.Status,
                        p.FirstName + ' ' + p.LastName as PatientName,
                        d.FirstName + ' ' + d.LastName as DoctorName,
                        c.Name as ClinicName,
                        d.DepartmentId,
                        t.Name as DepartmentName
                    FROM Appointments a
                    INNER JOIN Patients p
                        ON a.PatientId = p.PatientId
                    INNER JOIN Doctors d
                        ON a.DoctorId = d.DoctorId
                    INNER JOIN Clinics c
                        ON a.ClinicId = c.ClinicId
                    INNER JOIN Departments t
                        ON d.DepartmentId = t.DepartmentId
                    INNER JOIN Visits v
                        ON a.VisitId = v.VisitId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new Appointment
                            {
                                AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                                AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                                SlotTime = (TimeSpan)dr["SlotTime"],
                                Status = dr["Status"].ToString(),
                                PatientName = dr["PatientName"].ToString(),
                                DoctorName = dr["DoctorName"].ToString(),
                                ClinicName = dr["ClinicName"].ToString(),
                                VisitId = Convert.ToInt32(dr["VisitId"]),
                                DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                                DepartmentName = dr["DepartmentName"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }

        public Appointment Get(int appointmentId)
        {
            Appointment appointment = null;

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        a.AppointmentId,
                        a.AppointmentDate,
                        a.SlotTime,
                        v.VisitId,
                        a.Status,
                        p.FirstName + ' ' + p.LastName as PatientName,
                        d.FirstName + ' ' + d.LastName as DoctorName,
                        c.Name as ClinicName,
                        d.DepartmentId,
                        t.Name as DepartmentName
                    FROM Appointments a
                    INNER JOIN Patients p
                        ON a.PatientId = p.PatientId
                    INNER JOIN Doctors d
                        ON a.DoctorId = d.DoctorId
                    INNER JOIN Clinics c
                        ON a.ClinicId = c.ClinicId
                    INNER JOIN Visits v
                        ON a.VisitId = v.VisitId
                    INNER JOIN Departments t
                        ON d.DepartmentId = t.DepartmentId
                    WHERE a.AppointmentId = @AppointmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            appointment = new Appointment
                            {
                                AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                                AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                                SlotTime = (TimeSpan)dr["SlotTime"],
                                Status = dr["Status"].ToString(),
                                PatientName = dr["PatientName"].ToString(),
                                DoctorName = dr["DoctorName"].ToString(),
                                ClinicName = dr["ClinicName"].ToString(),
                                VisitId = Convert.ToInt32(dr["VisitId"]),
                                DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                                DepartmentName = dr["DepartmentName"].ToString()                                
                            };
                        }
                    }
                }
            }

            return appointment;
        }
        
        public List<Appointment> GetTodaysAppointments(int doctorId)
        {
            var list = new List<Appointment>();

            using var con = _db.GetConnection();

            string sql = @"
                SELECT
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.SlotTime,
                    v.VisitId,
                    a.Status,
                    p.FirstName + ' ' + p.LastName as PatientName,
                    d.FirstName + ' ' + d.LastName as DoctorName,
                    c.Name as ClinicName,
                    d.DepartmentId,
                    t.Name as DepartmentName
                FROM Appointments a
                INNER JOIN Patients p
                    ON a.PatientId = p.PatientId
                INNER JOIN Doctors d
                    ON a.DoctorId = d.DoctorId
                INNER JOIN Clinics c
                    ON a.ClinicId = c.ClinicId
                INNER JOIN Visits v
                    ON a.VisitId = v.VisitId
                INNER JOIN Departments t
                    ON d.DepartmentId = t.DepartmentId
                WHERE a.DoctorId = @DoctorId
                    AND CAST(a.AppointmentDate as date)=CAST(GETDATE() as date)
                ORDER BY a.SlotTime";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@DoctorId", doctorId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Appointment
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                    SlotTime = (TimeSpan)dr["SlotTime"],
                    Status = dr["Status"].ToString(),
                    PatientName = dr["PatientName"].ToString(),
                    DoctorName = dr["DoctorName"].ToString(),
                    ClinicName = dr["ClinicName"].ToString(),
                    VisitId = Convert.ToInt32(dr["VisitId"]),
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    DepartmentName = dr["DepartmentName"].ToString()
                });
            }

            return list;
        }

        public List<Appointment> ListByPatient(int patientId)
        {
            var list = new List<Appointment>();

            using var con = _db.GetConnection();

            string sql = @"
                SELECT
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.SlotTime,
                    a.Status,
                    p.FirstName + ' ' + p.LastName as PatientName,
                    d.FirstName + ' ' + d.LastName as DoctorName,
                    c.Name as ClinicName,
                    v.VisitId,
                    d.DepartmentId,
                    t.Name as DepartmentName
                FROM Appointments a
                INNER JOIN Patients p
                    ON a.PatientId = p.PatientId
                INNER JOIN Doctors d
                    ON a.DoctorId = d.DoctorId
                INNER JOIN Clinics c
                    ON a.ClinicId = c.ClinicId
                INNER JOIN Visits v
                    ON a.VisitId = v.VisitId
                INNER JOIN Departments t
                    ON d.DepartmentId = t.DepartmentId
                WHERE a.PatientId = @PatientId
                ORDER BY a.AppointmentDate DESC, a.SlotTime DESC";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Appointment
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                    SlotTime = (TimeSpan)dr["SlotTime"],
                    Status = dr["Status"].ToString(),
                    PatientName = dr["PatientName"].ToString(),
                    DoctorName = dr["DoctorName"].ToString(),
                    ClinicName = dr["ClinicName"].ToString(),
                    VisitId = Convert.ToInt32(dr["VisitId"]),
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    DepartmentName = dr["DepartmentName"].ToString()
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

