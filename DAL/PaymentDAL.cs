using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class PaymentDAL : BaseDAL, IDAL<Payment>
    {
        private readonly DbHelper _db;

        public PaymentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        // ------------------------------------------------------------
        // ADD PAYMENT
        // Supports both:
        //   Receipt -> BillId
        //   Refund  -> RefundId
        // ------------------------------------------------------------
        public int Add(Payment payment, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO Payments
                (PatientId, BillId, RefundId, ClinicId, PaymentType, PaymentDate, Amount, Mode, ReferenceNo)
                VALUES
                (@PatientId, @BillId, @RefundId, @ClinicId, @PaymentType, @PaymentDate, @Amount, @Mode, @ReferenceNo);

                SELECT SCOPE_IDENTITY();
            ";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@PatientId", payment.PatientId);
            cmd.Parameters.AddWithValue("@BillId", payment.BillId.HasValue ? payment.BillId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@RefundId", payment.RefundId.HasValue ? payment.RefundId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ClinicId", payment.ClinicId);
            cmd.Parameters.AddWithValue("@PaymentType", (int)payment.PaymentType);
            cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@Mode", payment.Mode ?? string.Empty);
            cmd.Parameters.AddWithValue("@ReferenceNo", payment.ReferenceNo ?? string.Empty);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ------------------------------------------------------------
        // GET PAYMENT BY ID
        // Works for both Bill and Refund payments.
        // ------------------------------------------------------------
        public Payment Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT p.PaymentId, p.PatientId, p.BillId, p.RefundId, p.ClinicId, p.PaymentDate, p.Amount, p.Mode,
                    p.ReferenceNo, p.CreatedAt,
                    COALESCE(b.PatientId, r.PatientId) AS PatientId,
                    CASE
                        WHEN p.BillId IS NOT NULL THEN 1
                        WHEN p.RefundId IS NOT NULL THEN 2
                        ELSE 0
                    END AS PaymentType
                FROM Payments p
                LEFT JOIN Bills b
                    ON p.BillId = b.BillId
                LEFT JOIN Refunds r
                    ON p.RefundId = r.RefundId
                WHERE p.PaymentId = @PaymentId
                  AND p.ClinicId = @ClinicId;
            ";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PaymentId", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (!rdr.Read())
                return null;

            return MapPayment(rdr);
        }

        // ------------------------------------------------------------
        // GET PAYMENTS BY BILL
        // ------------------------------------------------------------
        public List<Payment> GetPaymentsByBill(int billId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Payment> list = new();

            string query = @"
                SELECT
                    p.PaymentId,
                    p.BillId,
                    p.RefundId,
                    p.ClinicId,
                    p.PaymentDate,
                    p.Amount,
                    p.Mode,
                    p.ReferenceNo,
                    p.CreatedAt,
                    p.PatientId
                FROM Payments p
                INNER JOIN Bills b
                    ON p.BillId = b.BillId
                WHERE p.BillId = @BillId
                ORDER BY p.CreatedAt;
            ";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@BillId", billId);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(MapPayment(rdr, PaymentType.Receipt));
            }

            return list;
        }

        // ------------------------------------------------------------
        // GET PAYMENTS BY REFUND
        // ------------------------------------------------------------
        public List<Payment> GetPaymentsByRefund(int refundId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Payment> list = new();

            string query = @"
                SELECT
                    p.PaymentId,
                    p.BillId,
                    p.RefundId,
                    p.ClinicId,
                    p.PaymentDate,
                    p.Amount,
                    p.Mode,
                    p.ReferenceNo,
                    p.CreatedAt,
                    p.PatientId
                FROM Payments p
                INNER JOIN Refunds r
                    ON p.RefundId = r.RefundId
                WHERE p.RefundId = @RefundId
                ORDER BY p.CreatedAt;
            ";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@RefundId", refundId);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(MapPayment(rdr, PaymentType.Refund));
            }

            return list;
        }

        // ------------------------------------------------------------
        // TOTAL PAID AGAINST BILL
        // ------------------------------------------------------------
        public decimal GetTotalPaid(int billId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT ISNULL(SUM(Amount), 0)
                FROM Payments
                WHERE BillId = @BillId;
            ";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@BillId", billId);

            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        // ------------------------------------------------------------
        // TOTAL PAID AGAINST REFUND
        // ------------------------------------------------------------
        public decimal GetTotalRefunded(int refundId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT ISNULL(SUM(Amount), 0)
                FROM Payments
                WHERE RefundId = @RefundId;
            ";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@RefundId", refundId);

            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        // ------------------------------------------------------------
        // LIST ALL PAYMENTS FOR A PATIENT
        // Includes both receipts and refund payments.
        // ------------------------------------------------------------
        public List<Payment> ListByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Payment> list = new();

            string query = @"
                SELECT
                    p.PaymentId,
                    p.BillId,
                    p.RefundId,
                    p.ClinicId,
                    p.PaymentDate,
                    p.Amount,
                    p.Mode,
                    p.ReferenceNo,
                    p.CreatedAt,
                    p.PatientId,
                    CASE
                        WHEN p.BillId IS NOT NULL THEN 1
                        WHEN p.RefundId IS NOT NULL THEN 2
                        ELSE 0
                    END AS PaymentType
                FROM Payments p
                LEFT JOIN Bills b
                    ON p.BillId = b.BillId
                LEFT JOIN Refunds r
                    ON p.RefundId = r.RefundId
                WHERE p.PatientId = @PatientId
                ORDER BY p.CreatedAt;
            ";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(MapPayment(rdr));
            }

            return list;
        }

        // ------------------------------------------------------------
        // COMMON MAPPER
        // ------------------------------------------------------------
        private Payment MapPayment(SqlDataReader rdr, PaymentType? forcedPaymentType = null)
        {
            Payment payment = new Payment
            {
                PaymentId = Convert.ToInt32(rdr["PaymentId"]),
                BillId = GetNullableInt(rdr, "BillId"),
                RefundId = GetNullableInt(rdr, "RefundId"),
                ClinicId = Convert.ToInt32(rdr["ClinicId"]),
                PaymentDate = Convert.ToDateTime(rdr["PaymentDate"]),
                Amount = Convert.ToDecimal(rdr["Amount"]),
                Mode = GetNullableString(rdr, "Mode"),
                ReferenceNo = GetNullableString(rdr, "ReferenceNo"),
                PatientId = GetNullableInt(rdr, "PatientId") ?? 0,
                CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
            };

            if (forcedPaymentType.HasValue)
            {
                payment.PaymentType = forcedPaymentType.Value;
            }
            else if (rdr["PaymentType"] != DBNull.Value)
            {
                payment.PaymentType = (PaymentType)Convert.ToInt32(rdr["PaymentType"]);
            }

            return payment;
        }

        // ------------------------------------------------------------
        // NOT CURRENTLY USED
        // ------------------------------------------------------------
        public bool Update(Payment entity, SqlConnection conn, SqlTransaction tran)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            throw new NotImplementedException();
        }

        public List<Payment> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            throw new NotImplementedException();
        }
    }
}