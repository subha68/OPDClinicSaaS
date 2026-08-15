using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class PaymentDAL : IDAL<Payment>
    {
        private readonly DbHelper _db;

        public PaymentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Payment payment, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Payments
                (BillId, ClinicId, PaymentDate, Amount, Mode, ReferenceNo)
                VALUES
                (@BillId, @ClinicId, @PaymentDate, @Amount, @Mode, @ReferenceNo);
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@BillId", payment.BillId);
            cmd.Parameters.AddWithValue("@ClinicId", payment.ClinicId);
            cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@Mode", payment.Mode ?? "");
            cmd.Parameters.AddWithValue("@ReferenceNo", payment.ReferenceNo ?? "");

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public Payment Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT p.*, v.PatientId 
                                FROM Payments p 
                                INNER JOIN Bills b 
                                    ON p.BillId = b.BillId 
                                INNER JOIN Visits v 
                                    ON b.VisitId = v.VisitId 
                                WHERE p.PaymentId=@PaymentId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@PaymentId", id);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                return new Payment
                {
                    PaymentId = Convert.ToInt32(rdr["PaymentId"]),
                    BillId = Convert.ToInt32(rdr["BillId"]),
                    ClinicId = Convert.ToInt32(rdr["ClinicId"]),
                    PaymentDate = (DateTime)rdr["PaymentDate"],
                    Amount = Convert.ToDecimal(rdr["Amount"]),
                    Mode = rdr["Mode"].ToString(),
                    ReferenceNo = rdr["ReferenceNo"].ToString(),
                    PatientId = Convert.ToInt32(rdr["PatientId"]),
                    CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
                };
            }

            return null;
        }

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

        public List<Payment> GetPaymentsByBill(int billId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Payment> list = new();

            string query = @"
                SELECT p.*, v.PatientId
                FROM Payments p
                INNER JOIN Bills b ON p.BillId = b.BillId
                INNER JOIN Visits v ON b.VisitId = v.VisitId
                WHERE p.BillId=@BillId
                ORDER BY CreatedAt
            ";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@BillId", billId);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new Payment
                {
                    PaymentId = Convert.ToInt32(rdr["PaymentId"]),
                    BillId = Convert.ToInt32(rdr["BillId"]),
                    ClinicId = Convert.ToInt32(rdr["ClinicId"]),
                    PaymentDate = (DateTime)rdr["PaymentDate"],
                    Amount = Convert.ToDecimal(rdr["Amount"]),
                    Mode = rdr["Mode"].ToString(),
                    ReferenceNo = rdr["ReferenceNo"].ToString(),
                    PatientId = Convert.ToInt32(rdr["PatientId"]),
                    CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
                });
            }

            return list;
        }

        public decimal GetTotalPaid(int billId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT ISNULL(SUM(Amount),0)
                FROM Payments
                WHERE BillId=@BillId
            ";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@BillId", billId);

            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        public List<Payment> ListByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Payment> list = new List<Payment>();

            string query = @"
                SELECT p.*, v.PatientId
                FROM Payments p
                LEFT JOIN Bills b ON p.BillId = b.BillId
                INNER JOIN Visits v ON b.VisitId = v.VisitId
                WHERE v.PatientId=@PatientId
                ORDER BY CreatedAt
            ";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new Payment
                {
                    PaymentId = Convert.ToInt32(rdr["PaymentId"]),
                    BillId = Convert.ToInt32(rdr["BillId"]),
                    ClinicId = rdr["ClinicId"] != DBNull.Value ? Convert.ToInt32(rdr["ClinicId"]) : 0,
                    PaymentDate = (DateTime)rdr["PaymentDate"],
                    Amount = Convert.ToDecimal(rdr["Amount"]),
                    Mode = rdr["Mode"].ToString(),
                    ReferenceNo = rdr["ReferenceNo"] != DBNull.Value ? rdr["ReferenceNo"].ToString() : "",
                    PatientId = Convert.ToInt32(rdr["PatientId"]),
                    CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
                });
            }

            return list;
        }
    }
}