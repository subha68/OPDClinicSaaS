using Models;
using ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace DAL
{
    public class AppointmentDAL : IDAL<Appointment>
    {
        private readonly DbHelper _db;

        public AppointmentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Appointment model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO Appointments
                (
                    DoctorId,
                    ClinicId,
                    PatientId,
                    VisitId,
                    TokenNumber,
                    QueueNumber,
                    AppointmentDate,
                    SlotStartTime,
                    SlotEndTime,
                    Status
                )
                VALUES
                (
                    @DoctorId,
                    @ClinicId,
                    @PatientId,
                    @VisitId,
                    @TokenNumber,
                    @QueueNumber,
                    @AppointmentDate,
                    @SlotStartTime,
                    @SlotEndTime,
                    @Status
                );

                SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);
                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                cmd.Parameters.AddWithValue("@PatientId", model.PatientId);
                cmd.Parameters.AddWithValue("@VisitId", model.VisitId);
                cmd.Parameters.AddWithValue("@TokenNumber", model.TokenNumber);
                cmd.Parameters.AddWithValue("@QueueNumber", model.QueueNumber);
                cmd.Parameters.AddWithValue("@AppointmentDate", model.AppointmentDate);
                cmd.Parameters.AddWithValue("@SlotStartTime", model.SlotStartTime);
                cmd.Parameters.AddWithValue("@SlotEndTime", model.SlotEndTime);
                cmd.Parameters.AddWithValue("@Status", model.Status);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Update(Appointment model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE Appointments
                SET DoctorId = @DoctorId,
                    PatientId = @PatientId,
                    AppointmentDate = @AppointmentDate,
                    SlotStartTime = @SlotStartTime,
                    SlotEndTime = @SlotEndTime,
                    Status = @Status
                WHERE AppointmentId = @AppointmentId";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);
                cmd.Parameters.AddWithValue("@PatientId", model.PatientId);
                cmd.Parameters.AddWithValue("@AppointmentDate", model.AppointmentDate);
                cmd.Parameters.AddWithValue("@SlotStartTime", model.SlotStartTime);
                cmd.Parameters.AddWithValue("@SlotEndTime", model.SlotEndTime);
                cmd.Parameters.AddWithValue("@Status", model.Status);
                cmd.Parameters.AddWithValue("@AppointmentId", model.AppointmentId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = "DELETE FROM Appointments WHERE AppointmentId = @AppointmentId";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@AppointmentId", id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<Appointment> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Appointment>();

            string query = @"
                SELECT
                    a.AppointmentId,
                    a.AppointmentDate,
                    v.VisitId,
                    a.SlotStartTime,
                    a.SlotEndTime,
                    a.Status,
                    p.FirstName + ' ' + p.LastName as PatientName,
                    d.Salutation + ' ' + d.FirstName + ' ' + d.LastName as DoctorName,
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

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                        SlotTime = (TimeSpan)dr["SlotStartTime"],
                        //SlotEndTime = (TimeSpan)dr["SlotEndTime"],
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

            return list;
        }

        public Appointment Get(int appointmentId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            Appointment appointment = null;

            string query = @"
                SELECT
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.PatientId,
                    a.DoctorId,
                    a.SlotStartTime,
                    a.SlotEndTime,
                    v.VisitId,
                    a.TokenNumber,
                    a.QueueNumber,
                    a.Status,
                    p.FirstName + ' ' + p.LastName as PatientName,
                    d.Salutation + ' ' + d.FirstName + ' ' + d.LastName as DoctorName,
                    c.Name as ClinicName,
                    d.DepartmentId,
                    t.Name as DepartmentName,
                    CASE
                        WHEN b.BillCount > 0 THEN 'Billed'
                        ELSE 'Unbilled'
                    END AS BillStatus,

                    CASE
                        WHEN b.TotalNetAmount > 0 AND b.TotalPaidAmount = 0 THEN 'Due'
                        WHEN b.TotalBalance <= 0 THEN 'Paid'
                        WHEN b.TotalBalance < b.TotalNetAmount THEN 'Partially Paid'
                        ELSE 'None'
                    END AS PaymentStatus

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
                LEFT JOIN
                (
                    SELECT
                        VisitId,
                        COUNT(*) AS BillCount,
                        SUM(NetAmount) AS TotalNetAmount,
                        SUM(NetAmount - PaidAmount) AS TotalBalance,
                        SUM(PaidAmount) AS TotalPaidAmount
                    FROM Bills
                    GROUP BY VisitId
                ) b
                    ON a.VisitId = b.VisitId
                WHERE a.AppointmentId = @AppointmentId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    appointment = new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                        PatientId = Convert.ToInt32(dr["PatientId"]),
                        SlotTime = (TimeSpan)dr["SlotStartTime"],
                        //SlotEndTime = (TimeSpan)dr["SlotEndTime"],
                        Status = dr["Status"].ToString(),
                        BillingStatus = dr["BillStatus"].ToString(),
                        PaymentStatus = dr["PaymentStatus"].ToString(),
                        PatientName = dr["PatientName"].ToString(),
                        DoctorName = dr["DoctorName"].ToString(),
                        ClinicName = dr["ClinicName"].ToString(),
                        VisitId = Convert.ToInt32(dr["VisitId"]),
                        TokenNumber = dr["TokenNumber"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TokenNumber"]),
                        QueueNumber = dr["QueueNumber"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QueueNumber"]),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        DepartmentName = dr["DepartmentName"].ToString(),
                        DoctorId = Convert.ToInt32(dr["DoctorId"])
                    };
                }
            }

            return appointment;
        }
        
        public List<Appointment> GetTodaysAppointments(int doctorId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Appointment>();

            string sql = @"
                SELECT
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.PatientId,
                    a.SlotStartTime,
                    a.SlotEndTime,
                    v.VisitId,
                    a.TokenNumber,
                    a.QueueNumber,
                    a.Status,
                    p.FirstName + ' ' + p.LastName as PatientName,
                    d.Salutation + ' ' + d.FirstName + ' ' + d.LastName as DoctorName,
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
                ORDER BY a.SlotStartTime";

            var cmd = new SqlCommand(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@DoctorId", doctorId);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Appointment
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                    PatientId = Convert.ToInt32(dr["PatientId"]),
                    SlotTime = (TimeSpan)dr["SlotStartTime"],
                    //SlotEndTime = (TimeSpan)dr["SlotEndTime"],
                    Status = dr["Status"].ToString(),
                    PatientName = dr["PatientName"].ToString(),
                    DoctorName = dr["DoctorName"].ToString(),
                    ClinicName = dr["ClinicName"].ToString(),
                    VisitId = Convert.ToInt32(dr["VisitId"]),
                    TokenNumber = dr["TokenNumber"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TokenNumber"]),
                    QueueNumber = dr["QueueNumber"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QueueNumber"]),
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    DepartmentName = dr["DepartmentName"].ToString()
                });
            }

            return list;
        }

        public List<Appointment> GetAppointmentsByDate(int clinicId, int doctorId, DateTime appointmentDate, SqlConnection conn,
                                                            SqlTransaction? tran = null)
        {
            var list = new List<Appointment>();

            string sql = @"
                SELECT
                    a.AppointmentId,
                    a.VisitId,
                    a.SlotStartTime,
                    a.SlotEndTime,
                    a.TokenNumber,
                    a.QueueNumber,
                    a.Status,
                    a.CancellationReason,
                    p.PatientId,
                    p.FirstName + ' ' + p.LastName AS PatientName,

                    CASE
                        WHEN b.BillCount > 0 THEN 'Billed'
                        ELSE 'Unbilled'
                    END AS BillStatus,

                    CASE
                        WHEN b.TotalNetAmount > 0 AND b.TotalPaidAmount = 0 THEN 'Due'
                        WHEN b.TotalBalance <= 0 THEN 'Paid'
                        WHEN b.TotalBalance < b.TotalNetAmount THEN 'Partially Paid'
                        ELSE 'None'
                    END AS PaymentStatus

                FROM Appointments a

                INNER JOIN Visits v
                    ON a.VisitId = v.VisitId

                INNER JOIN Patients p
                    ON a.PatientId = p.PatientId

                INNER JOIN Doctors d
                    ON a.DoctorId = d.DoctorId

                INNER JOIN Clinics c
                    ON a.ClinicId = c.ClinicId

                LEFT JOIN
                (
                    SELECT
                        VisitId,
                        COUNT(*) AS BillCount,
                        SUM(NetAmount) AS TotalNetAmount,
                        SUM(NetAmount - PaidAmount) AS TotalBalance,
                        SUM(PaidAmount) AS TotalPaidAmount
                    FROM Bills
                    GROUP BY VisitId
                ) b
                    ON a.VisitId = b.VisitId

                WHERE a.DoctorId = @DoctorId
                AND a.ClinicId = @ClinicId
                AND a.AppointmentDate = @AppointmentDate
                ORDER BY a.SlotStartTime";

            using var cmd = new SqlCommand(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@DoctorId", doctorId);
            cmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Appointment
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    AppointmentDate = appointmentDate,
                    SlotTime = (TimeSpan)dr["SlotStartTime"],
                    TokenNumber = dr["TokenNumber"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TokenNumber"]),
                    QueueNumber = dr["QueueNumber"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QueueNumber"]),
                    Status = dr["Status"].ToString(),
                    CancellationReason = dr["CancellationReason"].ToString(),
                    PatientId = Convert.ToInt32(dr["PatientId"]),
                    PatientName = dr["PatientName"].ToString(),
                    BillingStatus = dr["BillStatus"].ToString(),
                    PaymentStatus = dr["PaymentStatus"].ToString()
                });
            }

            return list;
        }

        public List<Appointment> ListByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Appointment>();

            string sql = @"
                SELECT
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.SlotStartTime,
                    a.SlotEndTime,
                    a.Status,
                    p.FirstName + ' ' + p.LastName as PatientName,
                    d.Salutation + ' ' + d.FirstName + ' ' + d.LastName as DoctorName,
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
                ORDER BY a.AppointmentDate DESC, a.SlotStartTime DESC";

            using var cmd = new SqlCommand(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Appointment
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                    SlotTime = (TimeSpan)dr["SlotStartTime"],
                    //SlotEndTime = (TimeSpan)dr["SlotEndTime"],
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

        public void UpdateStatus(int appointmentId, string status, string cancellationReason, SqlConnection conn, SqlTransaction tran)
        {

            string sql = @"UPDATE Appointments 
                                SET Status=@Status, CancellationReason=@CancellationReason
                                WHERE AppointmentId=@AppointmentId";                

            using var cmd = new SqlCommand(sql, conn, tran);

            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@CancellationReason", cancellationReason ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            cmd.ExecuteNonQuery();
        }

        public int GetNextTokenNumber(int clinicId, DateTime appointmentDate, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                SELECT ISNULL(MAX(TokenNumber), 0) + 1
                FROM Appointments WITH (UPDLOCK, HOLDLOCK)
                WHERE ClinicId = @ClinicId
                AND AppointmentDate >= @AppointmentDate
                AND AppointmentDate < DATEADD(DAY, 1, @AppointmentDate);";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetNextQueueNumber(int clinicId, int doctorId, DateTime appointmentDate, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                SELECT ISNULL(MAX(QueueNumber), 0) + 1
                FROM Appointments WITH (UPDLOCK, HOLDLOCK)
                WHERE ClinicId = @ClinicId
                AND DoctorId = @DoctorId
                AND AppointmentDate >= @AppointmentDate
                AND AppointmentDate < DATEADD(DAY, 1, @AppointmentDate);";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@DoctorId", doctorId);
            cmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        
    }
}

