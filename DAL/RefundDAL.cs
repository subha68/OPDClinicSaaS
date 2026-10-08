using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class RefundDAL : BaseDAL, IDAL<Refund>
    {
        private readonly DbHelper _db;

        public RefundDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Refund refund, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO Refunds
                (RefundNo, BillId, ClinicId, VisitId, PatientId, RefundDate, RefundStatus, GrossAmount, DiscountAmount, 
                TaxAmount, NetAmount, RefundedAmount, Reason, RequestedBy, RequestedOn, CreatedOn)
                VALUES
                (@RefundNo, @BillId, @ClinicId, @VisitId, @PatientId, @RefundDate, @RefundStatus, @GrossAmount, @DiscountAmount, 
                @TaxAmount, @NetAmount, @RefundedAmount, @Reason, @RequestedBy, @RequestedOn, GETDATE());

                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundNo", refund.RefundNo);
            cmd.Parameters.AddWithValue("@BillId", refund.BillId);
            cmd.Parameters.AddWithValue("@ClinicId", refund.ClinicId);
            cmd.Parameters.AddWithValue("@VisitId", (object?)refund.VisitId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PatientId", (object?)refund.PatientId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RefundDate", refund.RefundDate);
            cmd.Parameters.AddWithValue("@RefundStatus", (int)refund.RefundStatus);
            cmd.Parameters.AddWithValue("@GrossAmount", refund.GrossAmount);
            cmd.Parameters.AddWithValue("@DiscountAmount", refund.DiscountAmount);
            cmd.Parameters.AddWithValue("@TaxAmount", refund.TaxAmount);
            cmd.Parameters.AddWithValue("@NetAmount", refund.NetAmount);
            cmd.Parameters.AddWithValue("@RefundedAmount", refund.RefundedAmount);
            cmd.Parameters.AddWithValue("@Reason", (object?)refund.Reason ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RequestedBy", (object?)refund.RequestedBy ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RequestedOn", (object?)refund.RequestedOn ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Refund refund, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE Refunds
                SET
                    GrossAmount = @GrossAmount,
                    DiscountAmount = @DiscountAmount,
                    TaxAmount = @TaxAmount,
                    NetAmount = @NetAmount,
                    RefundedAmount = @RefundedAmount,
                    Reason = @Reason
                WHERE RefundId = @RefundId";

            SqlCommand cmd = new(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundId", refund.RefundId);
            cmd.Parameters.AddWithValue("@GrossAmount", refund.GrossAmount);
            cmd.Parameters.AddWithValue("@DiscountAmount", refund.DiscountAmount);
            cmd.Parameters.AddWithValue("@TaxAmount", refund.TaxAmount);
            cmd.Parameters.AddWithValue("@NetAmount", refund.NetAmount);
            cmd.Parameters.AddWithValue("@RefundedAmount", refund.RefundedAmount);
            cmd.Parameters.AddWithValue("@Reason", (object?)refund.Reason ?? DBNull.Value);

            return cmd.ExecuteNonQuery() > 0;
        }

        public Refund? Get(int refundId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT r.*, b.BillNo, p.FirstName + ' ' + p.LastName as PatientName,
                        u1.Username as RequestedByName, u2.Username as ApprovedByName, u3.Username as CompletedByName
                FROM Refunds r
                INNER JOIN Bills b
                    ON r.BillId = b.BillId
                INNER JOIN Patients p
                    ON r.PatientId = p.PatientId
                LEFT JOIN Users u1
                    ON r.RequestedBy = u1.UserId
                LEFT JOIN Users u2
                    ON r.ApprovedBy = u2.UserId
                LEFT JOIN Users u3
                    ON r.CompletedBy = u3.UserId
                WHERE r.RefundId = @RefundId
                AND r.ClinicId = @ClinicId";

            SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@RefundId", refundId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapRefund(reader);
            }

            return null;
        }

        public List<Refund> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Refund> refunds = new();

            string query = @"
                SELECT r.*, b.BillNo, p.FirstName + ' ' + p.LastName as PatientName,
                        u1.Username as RequestedByName, u2.Username as ApprovedByName, u3.Username as CompletedByName
                FROM Refunds r
                INNER JOIN Bills b
                    ON r.BillId = b.BillId
                INNER JOIN Patients p
                    ON r.PatientId = p.PatientId
                LEFT JOIN Users u1
                    ON r.RequestedBy = u1.UserId
                LEFT JOIN Users u2
                    ON r.ApprovedBy = u2.UserId
                LEFT JOIN Users u3
                    ON r.CompletedBy = u3.UserId
                WHERE r.ClinicId = @ClinicId
                ORDER BY r.RefundDate DESC,
                         r.RefundId DESC";

            SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                refunds.Add(MapRefund(reader));
            }

            return refunds;
        }

        public bool Delete(int refundId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM Refunds
                WHERE RefundId = @RefundId
                AND ClinicId = @ClinicId";

            SqlCommand cmd = new(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundId", refundId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool SubmitForApproval(int refundId, int userId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE Refunds
                SET
                    RefundStatus = @RefundStatus,
                    RequestedBy = @RequestedBy,
                    RequestedOn = GETDATE()
                WHERE RefundId = @RefundId";

            SqlCommand cmd = new(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundId", refundId);
            cmd.Parameters.AddWithValue("@RefundStatus", (int)RefundStatus.PendingApproval);
            cmd.Parameters.AddWithValue("@RequestedBy", userId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Approve(int refundId, int approvedBy, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE Refunds
                SET
                    RefundStatus = @RefundStatus,
                    ApprovedBy = @ApprovedBy,
                    ApprovedOn = GETDATE()
                WHERE RefundId = @RefundId";

            SqlCommand cmd = new(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundId", refundId);
            cmd.Parameters.AddWithValue("@RefundStatus", (int)RefundStatus.Approved);
            cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Reject(int refundId, int rejectedBy, string reason, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE Refunds
                SET
                    RefundStatus = @RefundStatus,
                    RejectedBy = @RejectedBy,
                    RejectedOn = GETDATE(),
                    RejectionReason = @Reason
                WHERE RefundId = @RefundId";

            SqlCommand cmd = new(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundId", refundId);
            cmd.Parameters.AddWithValue("@RefundStatus", (int)RefundStatus.Rejected);
            cmd.Parameters.AddWithValue("@RejectedBy", rejectedBy);
            cmd.Parameters.AddWithValue("@Reason", reason);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateRefund(int refundId, decimal refundedAmount, RefundStatus refundStatus, int completedBy, 
                                    SqlConnection conn, SqlTransaction tran)
        {
            string query;

            if (refundStatus == RefundStatus.Completed)
            {
                query = @"
                    UPDATE Refunds
                    SET
                        RefundedAmount = @RefundedAmount,
                        RefundStatus = @RefundStatus,
                        CompletedBy = @CompletedBy,
                        CompletedOn = GETDATE()
                    WHERE RefundId = @RefundId;
                ";
            }
            else
            {
                query = @"
                    UPDATE Refunds
                    SET
                        RefundedAmount = @RefundedAmount,
                        RefundStatus = @RefundStatus,
                        CompletedBy = NULL,
                        CompletedOn = NULL
                    WHERE RefundId = @RefundId;
                ";
            }

            SqlCommand cmd = new(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundId", refundId);
            cmd.Parameters.AddWithValue("@RefundedAmount", refundedAmount);
            cmd.Parameters.AddWithValue("@RefundStatus", (int)refundStatus);
            cmd.Parameters.AddWithValue("@CompletedBy", completedBy);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Cancel(int refundId, int cancelledBy, string reason, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE Refunds
                SET
                    RefundStatus = @RefundStatus,
                    CancelledBy = @CancelledBy,
                    CancelledOn = GETDATE(),
                    Reason = @Reason
                WHERE RefundId = @RefundId";

            SqlCommand cmd = new(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundId", refundId);
            cmd.Parameters.AddWithValue("@RefundStatus", (int)RefundStatus.Cancelled);
            cmd.Parameters.AddWithValue("@CancelledBy", cancelledBy);
            cmd.Parameters.AddWithValue("@Reason", reason);

            return cmd.ExecuteNonQuery() > 0;
        }

        public List<Refund> ListByStatus(int clinicId, RefundStatus status, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Refund> refunds = new();

            string query = @"
                SELECT r.*, b.BillNo, p.FirstName + ' ' + p.LastName as PatientName,
                        u1.Username as RequestedByName, u2.Username as ApprovedByName, u3.Username as CompletedByName
                FROM Refunds r
                INNER JOIN Bills b
                    ON r.BillId = b.BillId
                INNER JOIN Patients p
                    ON r.PatientId = p.PatientId
                LEFT JOIN Users u1
                    ON r.RequestedBy = u1.UserId
                LEFT JOIN Users u2
                    ON r.ApprovedBy = u2.UserId
                LEFT JOIN Users u3
                    ON r.CompletedBy = u3.UserId
                WHERE r.ClinicId = @ClinicId
                AND r.RefundStatus = @RefundStatus
                ORDER BY r.RefundDate DESC,
                         r.RefundId DESC";

            SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@RefundStatus", (int)status);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                refunds.Add(MapRefund(reader));
            }

            return refunds;
        }

        public List<Refund> ListByPatient(int clinicId, int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Refund> refunds = new();

            string query = @"
                SELECT r.*, b.BillNo, p.FirstName + ' ' + p.LastName as PatientName,
                        u1.Username as RequestedByName, u2.Username as ApprovedByName, u3.Username as CompletedByName
                FROM Refunds r
                INNER JOIN Bills b
                    ON r.BillId = b.BillId
                INNER JOIN Patients p
                    ON r.PatientId = p.PatientId
                LEFT JOIN Users u1
                    ON r.RequestedBy = u1.UserId
                LEFT JOIN Users u2
                    ON r.ApprovedBy = u2.UserId
                LEFT JOIN Users u3
                    ON r.CompletedBy = u3.UserId
                WHERE r.ClinicId = @ClinicId
                AND r.PatientId = @PatientId
                ORDER BY r.RefundDate DESC,
                         r.RefundId DESC";

            SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                refunds.Add(MapRefund(reader));
            }

            return refunds;
        }

        public List<Refund> ListByBill(int clinicId, int billId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Refund> refunds = new();

            string query = @"
                SELECT r.*, b.BillNo, p.FirstName + ' ' + p.LastName as PatientName,
                        u1.Username as RequestedByName, u2.Username as ApprovedByName, u3.Username as CompletedByName
                FROM Refunds r
                INNER JOIN Bills b
                    ON r.BillId = b.BillId
                INNER JOIN Patients p
                    ON r.PatientId = p.PatientId
                LEFT JOIN Users u1
                    ON r.RequestedBy = u1.UserId
                LEFT JOIN Users u2
                    ON r.ApprovedBy = u2.UserId
                LEFT JOIN Users u3
                    ON r.CompletedBy = u3.UserId
                WHERE r.ClinicId = @ClinicId
                AND r.BillId = @BillId
                ORDER BY r.RefundDate DESC,
                         r.RefundId DESC";

            SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@BillId", billId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                refunds.Add(MapRefund(reader));
            }

            return refunds;
        }

        public List<Refund> ListApprovedNotCompleted(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Refund> refunds = new();

            string query = @"
                SELECT r.*, b.BillNo, p.FirstName + ' ' + p.LastName as PatientName,
                        u1.Username as RequestedByName, u2.Username as ApprovedByName, u3.Username as CompletedByName
                FROM Refunds r
                INNER JOIN Bills b
                    ON r.BillId = b.BillId
                INNER JOIN Patients p
                    ON r.PatientId = p.PatientId
                LEFT JOIN Users u1
                    ON r.RequestedBy = u1.UserId
                LEFT JOIN Users u2
                    ON r.ApprovedBy = u2.UserId
                LEFT JOIN Users u3
                    ON r.CompletedBy = u3.UserId
                WHERE r.ClinicId = @ClinicId
                AND r.RefundStatus IN (@ApprovedStatus, @PartialStatus)
                ORDER BY r.RefundDate DESC,
                         r.RefundId DESC";

            SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@ApprovedStatus", (int)RefundStatus.Approved);
            cmd.Parameters.AddWithValue("@PartialStatus", (int)RefundStatus.PartiallyRefunded);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                refunds.Add(MapRefund(reader));
            }

            return refunds;
        }

        private Refund MapRefund(SqlDataReader reader)
        {
            return new Refund
            {
                RefundId = Convert.ToInt32(reader["RefundId"]),
                BillId = Convert.ToInt32(reader["BillId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                RefundNo = GetNullableString(reader, "RefundNo"),
                VisitId = GetNullableInt(reader, "VisitId"),
                PatientId = GetNullableInt(reader, "PatientId"),
                RefundDate = Convert.ToDateTime(reader["RefundDate"]),
                RefundStatus = (RefundStatus)Convert.ToInt32(reader["RefundStatus"]),
                GrossAmount = Convert.ToDecimal(reader["GrossAmount"]),
                DiscountAmount = Convert.ToDecimal(reader["DiscountAmount"]),
                TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                RefundedAmount = Convert.ToDecimal(reader["RefundedAmount"]),
                Reason = GetNullableString(reader, "Reason"),
                RequestedBy = GetNullableInt(reader, "RequestedBy"),
                RequestedOn = GetNullableDate(reader, "RequestedOn"),
                ApprovedBy = GetNullableInt(reader, "ApprovedBy"),
                ApprovedOn = GetNullableDate(reader, "ApprovedOn"),
                RejectedBy = GetNullableInt(reader, "RejectedBy"),
                RejectedOn = GetNullableDate(reader, "RejectedOn"),
                RejectionReason = GetNullableString(reader, "RejectionReason"),
                CompletedBy = GetNullableInt(reader, "CompletedBy"),
                CompletedOn = GetNullableDate(reader, "CompletedOn"),
                PatientName = GetNullableString(reader, "PatientName"),
                BillNo = GetNullableString(reader, "BillNo"),
                RequestedByName = GetNullableString(reader, "RequestedByName"),
                ApprovedByName = GetNullableString(reader, "ApprovedByName"),
                CompletedByName = GetNullableString(reader, "CompletedByName")
            };
        }

    }
}