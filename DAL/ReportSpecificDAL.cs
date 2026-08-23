using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class ReportSpecificDAL
    {
        private readonly DbHelper _db;

        public ReportSpecificDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public List<DailyAppointmentSummaryRow> GetDailyAppointmentRows(DailyAppointmentSummaryFilter filter,
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            var rows = new List<DailyAppointmentSummaryRow>();

            const string sql = @"
                SELECT
                    a.SlotStartTime AS AppointmentTime,

                    LTRIM(RTRIM(
                        ISNULL(p.FirstName, '') + ' ' +
                        ISNULL(p.LastName, '')
                    )) AS PatientName,

                    p.PatientId AS PatientNumber,

                    LTRIM(RTRIM(
                        ISNULL(d.Salutation, '') + ' ' +
                        ISNULL(d.FirstName, '') + ' ' +
                        ISNULL(d.LastName, '')
                    )) AS DoctorName,

                    v.VisitType as AppointmentType,

                    a.Status as AppointmentStatus

                FROM Appointments a

                INNER JOIN Patients p
                    ON p.PatientId = a.PatientId

                INNER JOIN Visits v
                    ON v.VisitId = a.VisitId

                INNER JOIN Doctors d
                    ON d.DoctorId = a.DoctorId

                WHERE a.ClinicId = @ClinicId
                  AND CAST(a.AppointmentDate AS date) =
                      @AppointmentDate

                  AND (@DoctorId IS NULL
                        OR a.DoctorId = @DoctorId)

                ORDER BY
                    a.AppointmentDate,
                    a.SlotStartTime,
                    PatientName;
            ";

            using var cmd = new SqlCommand(sql, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@AppointmentDate", filter.AppointmentDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DoctorId", filter.DoctorId.HasValue ? filter.DoctorId.Value : DBNull.Value);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                rows.Add(new DailyAppointmentSummaryRow
                {
                    AppointmentTime = reader["AppointmentTime"] != DBNull.Value ? (TimeSpan)reader["AppointmentTime"] : null,
                    PatientName = reader["PatientName"]?.ToString() ?? "",
                    PatientNumber = reader["PatientNumber"]?.ToString() ?? "",
                    DoctorName = reader["DoctorName"]?.ToString() ?? "",
                    AppointmentType = reader["AppointmentType"]?.ToString() ?? "",
                    AppointmentStatus = reader["AppointmentStatus"]?.ToString() ?? ""
                });
            }

            return rows;
        }


        // ============================================================
        // SUMMARY
        // ============================================================

        public List<ReportSummaryItem> GetDailyAppointmentSummary(DailyAppointmentSummaryFilter filter,
                                                    SqlConnection conn, SqlTransaction? transaction)
        {
            List<ReportSummaryItem> list = new List<ReportSummaryItem>();

            const string sql = @"
                SELECT
                    COUNT(*) AS TotalAppointments,

                    SUM(
                        CASE
                            WHEN a.Status = 'Completed'
                            THEN 1
                            ELSE 0
                        END
                    ) AS CompletedAppointments,

                    SUM(
                        CASE
                            WHEN a.Status = 'Booked'
                            THEN 1
                            ELSE 0
                        END
                    ) AS BookedAppointments,

                    SUM(
                        CASE
                            WHEN a.Status = 'Cancelled'
                            THEN 1
                            ELSE 0
                        END
                    ) AS CancelledAppointments,

                    SUM(
                        CASE
                            WHEN a.Status = 'NoShow'
                            THEN 1
                            ELSE 0
                        END
                    ) AS NoShowAppointments

                FROM Appointments a

                WHERE a.ClinicId = @ClinicId
                  AND CAST(a.AppointmentDate AS date) =
                      @AppointmentDate

                  AND (
                        @DoctorId IS NULL
                        OR a.DoctorId = @DoctorId
                      );
            ";

            using var cmd = new SqlCommand(sql, conn, transaction);

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@AppointmentDate", filter.AppointmentDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DoctorId", filter.DoctorId.HasValue ? filter.DoctorId.Value : DBNull.Value);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return new List<ReportSummaryItem>();
            }

            int total = reader["TotalAppointments"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalAppointments"]);
            int completed = reader["CompletedAppointments"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompletedAppointments"]);
            int booked = reader["BookedAppointments"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BookedAppointments"]);
            int cancelled = reader["CancelledAppointments"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CancelledAppointments"]);
            int noShow = reader["NoShowAppointments"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NoShowAppointments"]);

            list.Add(new ReportSummaryItem("Total Appointments", total.ToString()));
            list.Add(new ReportSummaryItem("Completed", completed.ToString()));
            list.Add(new ReportSummaryItem("Booked", booked.ToString()));
            list.Add(new ReportSummaryItem("Cancelled", cancelled.ToString()));
            list.Add(new ReportSummaryItem("No-show", noShow.ToString()));
            
            return list;
        }

        public List<DoctorAppointmentSummaryRow> GetDoctorAppointmentRows(DoctorAppointmentSummaryFilter filter,
                                                                                SqlConnection conn,
                                                                                SqlTransaction? tran = null)
        {
            const string sql = @"
                SELECT
                    d.DoctorId,
                    d.Salutation + ' ' + d.FirstName + ' ' + d.LastName AS DoctorName,
                    COUNT(*) AS AppointmentCount,
                    SUM(CASE
                            WHEN a.Status = 'Completed'
                                THEN 1
                            ELSE 0
                        END
                    ) AS CompletedCount,
                    SUM(CASE
                            WHEN a.Status = 'Cancelled'
                                THEN 1
                            ELSE 0
                        END
                    ) AS CancelledCount,
                    SUM(CASE
                            WHEN a.Status = 'No-Show'
                                THEN 1
                            ELSE 0
                        END
                    ) AS NoShowCount
                FROM Appointments a
                INNER JOIN Doctors d
                    ON d.DoctorId = a.DoctorId
                    AND d.ClinicId = a.ClinicId
                WHERE
                    a.ClinicId = @ClinicId
                    AND a.AppointmentDate >= @DateFrom
                    AND a.AppointmentDate <= @DateTo
                    AND (@DoctorId IS NULL
                        OR a.DoctorId = @DoctorId)
                GROUP BY
                    d.DoctorId,
                    d.Salutation,
                    d.FirstName,
                    d.LastName
                ORDER BY DoctorName;
            ";

            var result = new List<DoctorAppointmentSummaryRow>();

            using SqlCommand cmd = new SqlCommand(sql, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DoctorId", filter.DoctorId.HasValue ? filter.DoctorId.Value : DBNull.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new DoctorAppointmentSummaryRow
                {
                    DoctorName = reader["DoctorName"]?.ToString() ?? "",
                    AppointmentCount = Convert.ToInt32(reader["AppointmentCount"]),
                    CompletedCount = Convert.ToInt32(reader["CompletedCount"]),
                    CancelledCount = Convert.ToInt32(reader["CancelledCount"]),
                    NoShowCount = Convert.ToInt32(reader["NoShowCount"])
                });
            }

            return result;
        }
        
        public List<PatientRegistrationSummaryRow> GetPatientRegistrationRows(PatientRegistrationSummaryFilter filter,
                                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PatientRegistrationSummaryRow> list = new List<PatientRegistrationSummaryRow>();

            string query = @"
                SELECT
                    CAST(p.CreatedAt AS DATE) AS RegistrationDate,
                    COUNT(*) AS RegistrationCount
                FROM Patients p
                WHERE p.CreatedAt >= @DateFrom
                AND p.CreatedAt < DATEADD(DAY, 1, @DateTo)
                GROUP BY CAST(p.CreatedAt AS DATE)
                ORDER BY RegistrationDate;
            ";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new PatientRegistrationSummaryRow
                {
                    RegistrationDate = Convert.ToDateTime(rdr["RegistrationDate"]),
                    RegistrationCount = Convert.ToInt32(rdr["RegistrationCount"])
                });
            }

            return list;
        }
        
        public List<DailyCollectionDataRow> GetDailyCollectionRows(DailyCollectionDataFilter filter,
                                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            List<DailyCollectionDataRow> list = new List<DailyCollectionDataRow>();

            string query = @"
                SELECT  COALESCE(p.CollectionDate, r.CollectionDate) AS CollectionDate,
                        COALESCE(p.PaymentMode, r.PaymentMode) AS PaymentMode,
                        ISNULL(p.TransactionCount, 0) AS TransactionCount,
                        ISNULL(p.CollectionAmount, 0) AS CollectionAmount,
                        ISNULL(r.RefundAmount, 0) AS RefundAmount
                FROM (SELECT    PaymentDate AS CollectionDate,
                                Mode AS PaymentMode,
                                COUNT(PaymentId) AS TransactionCount,
                                SUM(ISNULL(Amount, 0)) AS CollectionAmount
                        FROM Payments
                        WHERE ClinicId = @ClinicId
                            AND PaymentDate >= @DateFrom
                            AND PaymentDate < DATEADD(DAY, 1, @DateTo)
                            AND (@PaymentMode IS NULL
                                    OR @PaymentMode = ''
                                    OR Mode = @PaymentMode)
                        GROUP BY PaymentDate, Mode) as p
                FULL OUTER JOIN  (SELECT    RefundDate AS CollectionDate,
                                            Mode AS PaymentMode,
                                            SUM(ISNULL(Amount, 0)) AS RefundAmount
                                    FROM Refunds
                                    WHERE ClinicId = @ClinicId
                                        AND RefundDate >= @DateFrom
                                        AND RefundDate < DATEADD(DAY, 1, @DateTo)
                                        AND (@PaymentMode IS NULL
                                                OR @PaymentMode = ''
                                                OR Mode = @PaymentMode)
                                    GROUP BY RefundDate, Mode) as r
                    ON p.CollectionDate = r.CollectionDate
                    AND (p.PaymentMode = r.PaymentMode
                            OR (p.PaymentMode IS NULL AND r.PaymentMode IS NULL))
                ORDER BY COALESCE(p.CollectionDate, r.CollectionDate),
                        COALESCE(p.PaymentMode, r.PaymentMode);
            ";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@PaymentMode", string.IsNullOrWhiteSpace(filter.PaymentMode)
                                                                ? DBNull.Value
                                                                : filter.PaymentMode);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new DailyCollectionDataRow
                {
                    CollectionDate = Convert.ToDateTime(rdr["CollectionDate"]),
                    PaymentMode = rdr["PaymentMode"] != DBNull.Value ? rdr["PaymentMode"].ToString() ?? "" : "",
                    TransactionCount = Convert.ToInt32(rdr["TransactionCount"]),
                    CollectionAmount = Convert.ToDecimal(rdr["CollectionAmount"]),
                    RefundAmount = Convert.ToDecimal(rdr["RefundAmount"])
                });
            }

            return list;
        }

        public List<ReportSummaryItem> GetDailyCollectionSummary(DailyCollectionDataFilter filter,
                                                                    SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportSummaryItem> list = new();

            string query = @"
                SELECT
                    SUM(ISNULL(p.TransactionCount, 0)) AS TransactionCount,
                    SUM(ISNULL(p.CollectionAmount, 0)) AS CollectionAmount,
                    SUM(ISNULL(r.RefundAmount, 0)) AS RefundAmount,
                    SUM(ISNULL(p.CollectionAmount, 0))
                        - SUM(ISNULL(r.RefundAmount, 0)) AS NetCollection
                FROM
                (
                    SELECT
                        COUNT(PaymentId) AS TransactionCount,
                        SUM(ISNULL(Amount, 0)) AS CollectionAmount
                    FROM Payments
                    WHERE ClinicId = @ClinicId
                    AND PaymentDate >= @DateFrom
                    AND PaymentDate <= @DateTo
                    AND (
                            @PaymentMode IS NULL
                            OR @PaymentMode = ''
                            OR Mode = @PaymentMode
                        )
                ) p
                CROSS JOIN
                (
                    SELECT
                        SUM(ISNULL(Amount, 0)) AS RefundAmount
                    FROM Refunds
                    WHERE ClinicId = @ClinicId
                    AND RefundDate >= @DateFrom
                    AND RefundDate <= @DateTo
                    AND (
                            @PaymentMode IS NULL
                            OR @PaymentMode = ''
                            OR Mode = @PaymentMode
                        )
                ) r;
            ";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@PaymentMode", string.IsNullOrWhiteSpace(filter.PaymentMode)
                                                            ? DBNull.Value
                                                            : filter.PaymentMode);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                list.Add(new ReportSummaryItem("Transactions", rdr["TransactionCount"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Collections", rdr["CollectionAmount"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Refunds", rdr["RefundAmount"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Net Collection", rdr["NetCollection"].ToString() ?? "0"));
            }

            return list;
        }

        public List<OutstandingPatientBalanceRow> GetOutstandingPatientBalanceRows(OutstandingPatientBalanceFilter filter,
                                                                    SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<OutstandingPatientBalanceRow>();

            string query = @"
                    SELECT p.PatientId AS PatientNumber,
                            p.FirstName + ' ' + p.LastName AS PatientName,
                            b.BillNo AS InvoiceNumber,
                            b.BillDate AS InvoiceDate,
                            ISNULL(b.NetAmount, 0) AS InvoiceAmount,
                            ISNULL(pay.PaidAmount, 0) AS PaidAmount,
                            ISNULL(b.NetAmount, 0) - ISNULL(pay.PaidAmount, 0) AS OutstandingAmount
                    FROM Bills b
                    INNER JOIN Visits v
                        ON v.VisitId = b.VisitId
                    INNER JOIN Patients p
                        ON p.PatientId = v.PatientId
                    LEFT JOIN (SELECT BillId,
                                SUM(ISNULL(Amount, 0)) AS PaidAmount
                                FROM Payments
                                WHERE PaymentDate <= @AsOfDate
                                GROUP BY BillId) pay
                        ON pay.BillId = b.BillId
                    WHERE b.BillDate <= @AsOfDate
                    AND b.ClinicId = @ClinicId
                    AND (@PatientId IS NULL
                            OR p.PatientId = @PatientId)
                    AND (ISNULL(b.NetAmount, 0) - ISNULL(pay.PaidAmount, 0)) > 0
                    ORDER BY p.FirstName,
                            p.LastName,
                            b.BillDate,
                            b.BillNo;";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@AsOfDate", filter.AsOfDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@PatientId", filter.PatientId.HasValue ? filter.PatientId.Value : DBNull.Value);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new OutstandingPatientBalanceRow
                {
                    PatientNumber = reader["PatientNumber"]?.ToString() ?? "",
                    PatientName = reader["PatientName"]?.ToString() ?? "",
                    InvoiceNumber = reader["InvoiceNumber"]?.ToString() ?? "",
                    InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                    InvoiceAmount = Convert.ToDecimal(reader["InvoiceAmount"]),
                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                    OutstandingAmount = Convert.ToDecimal(reader["OutstandingAmount"])
                });
            }

            return list;
        }

        public List<ReportSummaryItem> GetOutstandingPatientBalanceSummary(OutstandingPatientBalanceFilter filter,
                                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportSummaryItem> list = new();

            string query = @"
                SELECT SUM(ISNULL(b.NetAmount, 0)) AS TotalBilled,
                        SUM(ISNULL(p.PaidAmount, 0)) AS TotalPaid,
                        SUM(ISNULL(b.NetAmount, 0)) - SUM(ISNULL(p.PaidAmount, 0)) AS TotalOutstanding
                FROM Bills b
                INNER JOIN Visits v
                    ON v.VisitId = b.VisitId
                INNER JOIN Patients pt
                    ON pt.PatientId = v.PatientId
                OUTER APPLY (SELECT SUM(ISNULL(pmt.Amount, 0)) AS PaidAmount
                                FROM Payments pmt
                                WHERE pmt.BillId = b.BillId
                                AND pmt.PaymentDate <= @AsOfDate) p
                WHERE b.ClinicId = @ClinicId
                AND b.BillDate <= @AsOfDate
                AND (@PatientId IS NULL
                    OR pt.PatientId = @PatientId)
                AND (ISNULL(b.NetAmount, 0) - ISNULL(p.PaidAmount, 0)) > 0;";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@AsOfDate", filter.AsOfDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@PatientId", filter.PatientId.HasValue ? filter.PatientId.Value : DBNull.Value);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                list.Add(new ReportSummaryItem("Total Billed", rdr["TotalBilled"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Total Paid", rdr["TotalPaid"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Total Outstanding", rdr["TotalOutstanding"].ToString() ?? "0"));
            }

            return list;
        }

        public List<CancelledNoshowAppointmentRow> GetCancelledNoshowAppointmentRows(CancelledNoshowAppointmentFilter filter,
                                                                    SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<CancelledNoshowAppointmentRow>();

            string query = @"
                        SELECT a.AppointmentDate AS AppointmentDate,
                                a.SlotStartTime AS AppointmentTime,
                                p.FirstName + ' ' + p.LastName AS PatientName,
                                ISNULL(d.Salutation, 'Dr.') + ' ' + d.FirstName + ' ' + d.LastName AS DoctorName,
                                a.Status AS AppointmentStatus,
                                a.CancellationReason AS Remarks
                        FROM Appointments a
                        INNER JOIN Doctors d
                            ON d.DoctorId = a.DoctorId
                        INNER JOIN Patients p
                            ON p.PatientId = a.PatientId
                        WHERE a.ClinicId = @ClinicId
                        AND a.AppointmentDate >= @DateFrom
                        AND a.AppointmentDate <= @DateTo
                        AND a.Status IN ('Cancelled', 'No-show')
                        AND (@DoctorId IS NULL OR a.DoctorId = @DoctorId)
                        AND (@AppointmentStatus IS NULL
                            OR @AppointmentStatus = ''
                            OR a.Status = @AppointmentStatus)
                        ORDER BY a.AppointmentDate,
                                a.SlotStartTime,
                                a.TokenNumber;";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DoctorId", filter.DoctorId.HasValue ? filter.DoctorId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@AppointmentStatus", string.IsNullOrWhiteSpace(filter.AppointmentStatus)
                                                            ? DBNull.Value
                                                            : filter.AppointmentStatus);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new CancelledNoshowAppointmentRow
                {
                    AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]),
                    AppointmentTime = (TimeSpan)reader["AppointmentTime"],
                    PatientName = reader["PatientName"].ToString(),
                    DoctorName = reader["DoctorName"].ToString(),
                    AppointmentStatus = reader["AppointmentStatus"].ToString(),
                    Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() ?? "" : ""
                });
            }

            return list;
        }

        public List<ReportSummaryItem> GetCancelledNoshowAppointmentSummary(CancelledNoshowAppointmentFilter filter,
                                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportSummaryItem> list = new();

            string query = @"
                SELECT
                    SUM(CASE WHEN a.Status = 'Cancelled' THEN 1 ELSE 0 END) AS TotalCancelled,
                    SUM(CASE WHEN a.Status = 'No-show' THEN 1 ELSE 0 END) AS TotalNoShow,
                    COUNT(*) AS TotalCancelledNoshow
                FROM Appointments a
                INNER JOIN Doctors d
                    ON d.DoctorId = a.DoctorId
                WHERE a.ClinicId = @ClinicId
                AND a.AppointmentDate >= @DateFrom
                AND a.AppointmentDate <= @DateTo
                AND a.Status IN ('Cancelled', 'No-show')
                AND (@DoctorId IS NULL
                    OR a.DoctorId = @DoctorId)
                AND (@AppointmentStatus IS NULL
                    OR @AppointmentStatus = ''
                    OR a.Status = @AppointmentStatus);";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DoctorId", filter.DoctorId.HasValue ? filter.DoctorId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@AppointmentStatus", string.IsNullOrWhiteSpace(filter.AppointmentStatus)
                                                            ? DBNull.Value
                                                            : filter.AppointmentStatus);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                list.Add(new ReportSummaryItem("Total Cancelled", rdr["TotalCancelled"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Total No-show", rdr["TotalNoShow"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Total Cancelled / No-show", rdr["TotalCancelledNoshow"].ToString() ?? "0"));
            }

            return list;
        }

        public List<DoctorRevenueRow> GetDoctorRevenueRows(DoctorRevenueFilter filter,
                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<DoctorRevenueRow>();

            string query = @"
                SELECT
                    ISNULL(d.Salutation, 'Dr.') + ' ' + d.FirstName + ' ' + d.LastName AS DoctorName,
                    COUNT(DISTINCT b.VisitId) AS ConsultationCount,
                    SUM(ISNULL(b.GrossAmount, 0)) AS GrossAmount,
                    SUM(ISNULL(b.DiscountAmount, 0)) AS DiscountAmount,
                    SUM(ISNULL(b.NetAmount, 0)) AS NetAmount
                FROM Bills b
                INNER JOIN Visits v
                    ON v.VisitId = b.VisitId
                INNER JOIN Appointments a
                    ON a.VisitId = v.VisitId
                INNER JOIN Doctors d
                    ON d.DoctorId = a.DoctorId
                WHERE b.ClinicId = @ClinicId
                AND b.BillDate >= @DateFrom
                AND b.BillDate <= @DateTo
                AND (@DoctorId IS NULL
                    OR d.DoctorId = @DoctorId)
                GROUP BY d.DoctorId,
                    d.Salutation,
                    d.FirstName,
                    d.LastName
                ORDER BY d.FirstName,
                    d.LastName;";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DoctorId", filter.DoctorId.HasValue ? filter.DoctorId.Value : DBNull.Value);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new DoctorRevenueRow
                {
                    DoctorName = reader["DoctorName"].ToString() ?? "",
                    ConsultationCount = Convert.ToInt32(reader["ConsultationCount"]),
                    GrossAmount = Convert.ToDecimal(reader["GrossAmount"]),
                    DiscountAmount = Convert.ToDecimal(reader["DiscountAmount"]),
                    NetAmount = Convert.ToDecimal(reader["NetAmount"])
                });
            }

            return list;
        }

        public List<ReportSummaryItem> GetDoctorRevenueSummary(DoctorRevenueFilter filter,
                                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportSummaryItem> list = new();

            string query = @"
                SELECT
                    COUNT(DISTINCT b.VisitId) AS TotalConsultations,
                    SUM(ISNULL(b.GrossAmount, 0)) AS TotalGrossAmount,
                    SUM(ISNULL(b.DiscountAmount, 0)) AS TotalDiscountAmount,
                    SUM(ISNULL(b.NetAmount, 0)) AS TotalNetAmount
                FROM Bills b
                INNER JOIN Visits v
                    ON v.VisitId = b.VisitId
                INNER JOIN Appointments a
                    ON a.VisitId = v.VisitId
                INNER JOIN Doctors d
                    ON d.DoctorId = a.DoctorId
                WHERE b.ClinicId = @ClinicId
                AND b.BillDate >= @DateFrom
                AND b.BillDate <= @DateTo
                AND (@DoctorId IS NULL
                    OR d.DoctorId = @DoctorId);";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DoctorId", filter.DoctorId.HasValue ? filter.DoctorId.Value : DBNull.Value);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                list.Add(new ReportSummaryItem("Total Consultations", rdr["TotalConsultations"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Total Gross Amount", rdr["TotalGrossAmount"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Total Discount", rdr["TotalDiscountAmount"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Total Net Amount", rdr["TotalNetAmount"].ToString() ?? "0"));
            }

            return list;
        }

        public List<PatientVisitHistoryRow> GetPatientVisitHistoryRows(PatientVisitHistoryFilter filter,
                                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<PatientVisitHistoryRow>();

            string query = @"
                SELECT
                    v.VisitDate AS VisitDate,
                    ISNULL(d.Salutation, 'Dr.') + ' ' + d.FirstName + ' ' + d.LastName AS DoctorName,
                    v.VisitType AS VisitType,
                    ISNULL(diag.Diagnosis, '') AS Diagnosis,
                    ISNULL(b.NetAmount, 0) AS Amount
                FROM Visits v
                INNER JOIN Appointments a
                    ON a.VisitId = v.VisitId
                INNER JOIN Doctors d
                    ON d.DoctorId = a.DoctorId
                OUTER APPLY
                (SELECT STRING_AGG(NULLIF(LTRIM(RTRIM(cf.FieldValue)), ''), ', ') AS Diagnosis
                    FROM Consultations c
                    INNER JOIN ConsultationSections cs
                        ON cs.ConsultationId = c.ConsultationId
                    INNER JOIN ConsultationRows cr
                        ON cr.SectionId = cs.SectionId
                    INNER JOIN ConsultationFields cf
                        ON cf.RowId = cr.RowId
                    WHERE c.AppointmentId = a.AppointmentId
                    AND c.ClinicId = @ClinicId
                    AND cs.ClinicId = @ClinicId
                    AND cr.ClinicId = @ClinicId
                    AND cf.ClinicId = @ClinicId
                    AND cf.FieldCode = 'DIAGNOSIS') diag
                OUTER APPLY
                (SELECT SUM(ISNULL(bi.NetAmount, 0)) AS NetAmount
                    FROM Bills bi
                    WHERE bi.VisitId = v.VisitId
                    AND bi.ClinicId = @ClinicId) b
                WHERE v.ClinicId = @ClinicId
                AND v.PatientId = @PatientId
                AND (@DateFrom IS NULL OR v.VisitDate >= @DateFrom)
                AND (@DateTo IS NULL OR v.VisitDate <= @DateTo)
                ORDER BY v.VisitDate DESC,
                    a.SlotStartTime DESC,
                    v.VisitId DESC;";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@PatientId", filter.PatientId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.HasValue 
                                                        ? filter.DateFrom.Value.ToDateTime(TimeOnly.MinValue)
                                                        : DBNull.Value);
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.HasValue 
                                                        ? filter.DateTo.Value.ToDateTime(TimeOnly.MinValue)
                                                        : DBNull.Value);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new PatientVisitHistoryRow
                {
                    VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                    DoctorName = reader["DoctorName"].ToString() ?? "",
                    VisitType = reader["VisitType"].ToString() ?? "",
                    Diagnosis = reader["Diagnosis"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"])
                });
            }

            return list;
        }

        public List<ReportSummaryItem> GetPatientVisitHistorySummary(PatientVisitHistoryFilter filter,
                                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportSummaryItem> list = new();

            string query = @"
                SELECT
                    COUNT(DISTINCT v.VisitId) AS TotalVisits,
                    ISNULL(SUM(ISNULL(b.NetAmount, 0)), 0) AS TotalAmount
                FROM Visits v
                OUTER APPLY
                (
                    SELECT SUM(ISNULL(bi.NetAmount, 0)) AS NetAmount
                    FROM Bills bi
                    WHERE bi.VisitId = v.VisitId
                    AND bi.ClinicId = @ClinicId
                ) b
                WHERE v.ClinicId = @ClinicId
                AND v.PatientId = @PatientId
                AND (@DateFrom IS NULL OR v.VisitDate >= @DateFrom)
                AND (@DateTo IS NULL OR v.VisitDate <= @DateTo);";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@PatientId", filter.PatientId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.HasValue 
                                                        ? filter.DateFrom.Value.ToDateTime(TimeOnly.MinValue)
                                                        : DBNull.Value);
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.HasValue 
                                                        ? filter.DateTo.Value.ToDateTime(TimeOnly.MinValue)
                                                        : DBNull.Value);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                list.Add(new ReportSummaryItem("Total Visits", rdr["TotalVisits"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Total Amount", rdr["TotalAmount"].ToString() ?? "0"));
            }

            return list;
        }

    }
}