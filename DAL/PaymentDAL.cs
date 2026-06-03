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

        public int Add(Payment payment)
        {
            using SqlConnection con = _db.GetConnection();

            string query = @"INSERT INTO Payments
                (BillId, Amount, Mode, ReferenceNo)
                VALUES
                (@BillId, @Amount, @Mode, @ReferenceNo);
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@BillId", payment.BillId);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@Mode", payment.Mode ?? "");
            cmd.Parameters.AddWithValue("@ReferenceNo", payment.ReferenceNo ?? "");

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public Payment Get(int id)
        {
            using SqlConnection con = _db.GetConnection();

            string query = @"SELECT p.*, v.PatientId 
                                FROM Payments p 
                                INNER JOIN Bills b 
                                    ON p.BillId = b.BillId 
                                INNER JOIN Visits v 
                                    ON b.VisitId = v.VisitId 
                                WHERE p.PaymentId=@PaymentId";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@PaymentId", id);

            con.Open();

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                return new Payment
                {
                    PaymentId = Convert.ToInt32(rdr["PaymentId"]),
                    BillId = Convert.ToInt32(rdr["BillId"]),
                    Amount = Convert.ToDecimal(rdr["Amount"]),
                    Mode = rdr["Mode"].ToString(),
                    ReferenceNo = rdr["ReferenceNo"].ToString(),
                    PatientId = Convert.ToInt32(rdr["PatientId"]),
                    CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
                };
            }

            return null;
        }

        public bool Update(Payment entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Payment> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Payment> GetPaymentsByBill(int billId)
        {
            List<Payment> list = new();

            using SqlConnection con = _db.GetConnection();

            string query = @"
                SELECT p.*, v.PatientId
                FROM Payments p
                INNER JOIN Bills b ON p.BillId = b.BillId
                INNER JOIN Visits v ON b.VisitId = v.VisitId
                WHERE p.BillId=@BillId
                ORDER BY CreatedAt
            ";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@BillId", billId);

            con.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new Payment
                {
                    PaymentId = Convert.ToInt32(rdr["PaymentId"]),
                    BillId = Convert.ToInt32(rdr["BillId"]),
                    Amount = Convert.ToDecimal(rdr["Amount"]),
                    Mode = rdr["Mode"].ToString(),
                    ReferenceNo = rdr["ReferenceNo"].ToString(),
                    PatientId = Convert.ToInt32(rdr["PatientId"]),
                    CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
                });
            }

            return list;
        }

        public decimal GetTotalPaid(int billId)
        {
            using SqlConnection con = _db.GetConnection();

            string query = @"
                SELECT ISNULL(SUM(Amount),0)
                FROM Payments
                WHERE BillId=@BillId
            ";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@BillId", billId);

            con.Open();

            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        public List<Payment> ListByPatient(int patientId)
        {
            List<Payment> list = new();

            using SqlConnection con = _db.GetConnection();

            string query = @"
                SELECT p.*, v.PatientId
                FROM Payments p
                INNER JOIN Bills b ON p.BillId = b.BillId
                INNER JOIN Visits v ON b.VisitId = v.VisitId
                WHERE v.PatientId=@PatientId
                ORDER BY CreatedAt
            ";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            con.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new Payment
                {
                    PaymentId = Convert.ToInt32(rdr["PaymentId"]),
                    BillId = Convert.ToInt32(rdr["BillId"]),
                    Amount = Convert.ToDecimal(rdr["Amount"]),
                    Mode = rdr["Mode"].ToString(),
                    ReferenceNo = rdr["ReferenceNo"].ToString(),
                    PatientId = Convert.ToInt32(rdr["PatientId"]),
                    CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
                });
            }

            return list;
        }
    }
}