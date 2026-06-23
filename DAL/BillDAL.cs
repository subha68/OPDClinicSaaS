using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class BillDAL : IDAL<Bill>
    {
        private readonly DbHelper _db;

        public BillDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Bill bill, SqlConnection conn, SqlTransaction tran)
        {
            int billId = 0;

            string query = @"INSERT INTO Bills (VisitId, BillDate, GrossAmount, DiscountAmount, NetAmount, PaidAmount) 
                                VALUES (@visitId, @billDate, @grossAmount, @discountAmount, @netAmount, @paidAmount);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@visitId", bill.VisitId);
            cmd.Parameters.AddWithValue("@billDate", bill.BillDate);
            cmd.Parameters.AddWithValue("@grossAmount", bill.GrossAmount);
            cmd.Parameters.AddWithValue("@discountAmount", bill.DiscountAmount);
            cmd.Parameters.AddWithValue("@netAmount", bill.NetAmount);
            cmd.Parameters.AddWithValue("@paidAmount", 0); // Assuming new bills are unpaid
            //cmd.Parameters.AddWithValue("@clinicId", bill.ClinicId);

            billId = Convert.ToInt32(cmd.ExecuteScalar());

            return billId;
        }

        public bool Update(Bill bill, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Bills
                                SET PatientId = @patientId,
                                    VisitId = @visitId,
                                    BillDate = @billDate,
                                    GrossAmount = @grossAmount,
                                    DiscountAmount = @discountAmount,
                                    NetAmount = @netAmount,
                                    PaidAmount = @paidAmount
                                WHERE BillId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@patientId", bill.PatientId);
            cmd.Parameters.AddWithValue("@visitId", bill.VisitId);
            cmd.Parameters.AddWithValue("@billDate", bill.BillDate);
            cmd.Parameters.AddWithValue("@grossAmount", bill.GrossAmount);
            cmd.Parameters.AddWithValue("@discountAmount", bill.DiscountAmount);
            cmd.Parameters.AddWithValue("@netAmount", bill.NetAmount);
            cmd.Parameters.AddWithValue("@paidAmount", bill.TotalPaidAmount);
            cmd.Parameters.AddWithValue("@id", bill.BillId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool UpdatePayment(int billId, decimal totalPaid, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Bills
                                SET PaidAmount = @paidAmount
                                WHERE BillId = @billId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@paidAmount", totalPaid);
            cmd.Parameters.AddWithValue("@billId", billId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }


        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Bills WHERE Id = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public Bill Get(int id, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT b.*,
                                    v.PatientId,
                                    v.VisitDate 
                            FROM Bills b 
                            JOIN Visits v ON b.VisitId = v.VisitId
                            WHERE b.BillId = @id";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            var bill = new Bill();

            if (reader.Read())
            {
                return new Bill
                {
                    BillId = (int)reader["BillId"],
                    PatientId = (int)reader["PatientId"],
                    VisitId = (int)reader["VisitId"],
                    VisitDate = (DateTime)reader["VisitDate"],
                    BillDate = (DateTime)reader["BillDate"],
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                };
            }

            return null;
        }

        public List<Bill> GetAll(SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Bill>();

            string query = @"SELECT b.*, v.VisitDate 
                                FROM Bills b 
                                JOIN Visits v 
                                    ON b.VisitId = v.VisitId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Bill
                {
                    BillId = (int)reader["BillId"],
                    VisitId = (int)reader["VisitId"],
                    VisitDate = (DateTime)reader["VisitDate"],
                    BillDate = (DateTime)reader["BillDate"],
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                });
            }

            return list;

        }

        public List<Bill> GetAllByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Bill>();

            string query = @"SELECT b.*, v.VisitDate, p.PatientId, p.FirstName + ' ' + p.LastName as PatientName
                                FROM Bills b 
                                JOIN Visits v 
                                    ON b.VisitId = v.VisitId
                                JOIN Patients p
                                    ON v.PatientId = p.PatientId 
                                WHERE v.ClinicId = @clinicId
                                ORDER BY b.BillDate DESC";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@clinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Bill
                {
                    BillId = (int)reader["BillId"],
                    VisitId = (int)reader["VisitId"],
                    VisitDate = (DateTime)reader["VisitDate"],
                    PatientId = (int)reader["PatientId"],
                    PatientName = reader["PatientName"].ToString(),
                    BillDate = (DateTime)reader["BillDate"],
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                });
            }

            return list;

        }

        public List<Bill> ListByPatient(int clinicId, int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Bill>();

            string query = @"SELECT b.*, p.PatientId, p.FirstName + ' ' + p.LastName as PatientName, v.VisitDate
                                FROM Bills b
                                JOIN Visits v ON b.VisitId = v.VisitId
                                JOIN Patients p ON v.PatientId = p.PatientId
                                WHERE p.PatientId = @patientId
                                AND v.ClinicId = @clinicId
                                ORDER BY b.BillDate DESC";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@patientId", patientId);
            cmd.Parameters.AddWithValue("@clinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Bill
                {
                    BillId = (int)reader["BillId"],
                    PatientId = (int)reader["PatientId"],
                    VisitId = (int)reader["VisitId"],
                    VisitDate = (DateTime)reader["VisitDate"],
                    BillDate = (DateTime)reader["BillDate"],
                    PatientName = reader["PatientName"].ToString(),
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                });
            }

            return list;
        }

    }

}