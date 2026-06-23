using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class BillLineDAL : IDAL<BillLine>
    {
        private readonly DbHelper _db;

        public BillLineDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(BillLine line, SqlConnection conn, SqlTransaction tran)
        {
            var query = @"INSERT INTO BillLines (BillId, ReferenceId, Rate, Quantity, GrossAmount, DiscountPercent, DiscountAmount, NetAmount)
                            VALUES (@BillId, @ReferenceId, @Rate, @Quantity, @GrossAmount, @DiscountPercent, @DiscountAmount, @NetAmount);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@BillId", line.BillId);
            cmd.Parameters.AddWithValue("@ReferenceId", line.ReferenceId);
            cmd.Parameters.AddWithValue("@Rate", line.Rate);
            cmd.Parameters.AddWithValue("@Quantity", line.Quantity);
            cmd.Parameters.AddWithValue("@GrossAmount", line.GrossAmount);
            cmd.Parameters.AddWithValue("@DiscountPercent", line.DiscountPercent);
            cmd.Parameters.AddWithValue("@DiscountAmount", line.DiscountAmount);
            cmd.Parameters.AddWithValue("@NetAmount", line.NetAmount);

            return Convert.ToInt32(Convert.ToInt32(cmd.ExecuteScalar()));
        }

        public BillLine Get(int id, SqlConnection conn, SqlTransaction? tran = null)
        {
            var query = @"SELECT l.*, v.PatientId, bc.ChargeCode, bc.Description
                                FROM BillLines l 
                                JOIN Bills b 
                                    ON l.BillId = b.BillId
                                JOIN Visits v 
                                    ON b.VisitId = v.VisitId
                                JOIN BillingCharges bc 
                                    ON l.ChargeId = bc.ChargeId
                                WHERE l.LineId = @LineId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@LineId", id);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new BillLine
                    {
                        LineId = Convert.ToInt32(reader["LineId"]),
                        BillId = Convert.ToInt32(reader["BillId"]),
                        ReferenceId = Convert.ToInt32(reader["ChargeId"]),
                        Rate = Convert.ToDecimal(reader["Rate"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                        PatientId = Convert.ToInt32(reader["PatientId"]),
                        //ChargeCode = reader["ChargeCode"].ToString(),
                        Description = reader["Description"].ToString()
                    };
                }
            }

            return null;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            var query = "DELETE FROM BillLines WHERE LineId = @LineId";
            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@LineId", id);

            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Update(BillLine model, SqlConnection conn, SqlTransaction tran)
        {
            var query = @"UPDATE BillLines SET 
                            BillId = @BillId, 
                            ReferenceId = @ReferenceId, 
                            Rate = @Rate, 
                            Quantity = @Quantity, 
                            DiscountPercent = @DiscountPercent
                            WHERE LineId = @LineId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@BillId", model.BillId);
            cmd.Parameters.AddWithValue("@ChargeId", model.ReferenceId);
            cmd.Parameters.AddWithValue("@Rate", model.Rate);
            cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
            cmd.Parameters.AddWithValue("@DiscountPercent", model.DiscountPercent);
            cmd.Parameters.AddWithValue("@LineId", model.LineId);

            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public List<BillLine> GetAll(SqlConnection conn, SqlTransaction? tran = null)
        {
            List<BillLine> lines = new List<BillLine>();

            var query = @"SELECT l.*, v.PatientId 
                            FROM BillLines l 
                            JOIN Bills b 
                                ON l.BillId = b.BillId
                            JOIN Visits v 
                                ON b.VisitId = v.VisitId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lines.Add(new BillLine
                    {
                        LineId = Convert.ToInt32(reader["LineId"]),
                        BillId = Convert.ToInt32(reader["BillId"]),
                        ReferenceId = Convert.ToInt32(reader["ReferenceId"]),
                        Rate = Convert.ToDecimal(reader["Rate"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                        PatientId = Convert.ToInt32(reader["PatientId"])
                    });
                }
            }

            return lines;
        }

        public List<BillLine> GetByBillId(int billId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<BillLine> lines = new List<BillLine>();

            var query = @"SELECT l.*, bc.ChargeCode, bc.Description, v.PatientId 
                            FROM BillLines l 
                            JOIN Bills b 
                                ON l.BillId = b.BillId 
                            JOIN Visits v 
                                ON b.VisitId = v.VisitId
                            JOIN BillingCharges bc 
                                ON l.ReferenceId = bc.ChargeId
                            WHERE l.BillId = @BillId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@BillId", billId);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lines.Add(new BillLine
                    {
                        LineId = Convert.ToInt32(reader["LineId"]),
                        BillId = Convert.ToInt32(reader["BillId"]),
                        ReferenceId = Convert.ToInt32(reader["ReferenceId"]),
                        Description = reader["Description"].ToString(),
                        Rate = Convert.ToDecimal(reader["Rate"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                        PatientId = Convert.ToInt32(reader["PatientId"])
                    });
                }
            }

            return lines;
        }

        public List<BillLine> ListByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<BillLine> lines = new List<BillLine>();

            var query = @"SELECT l.*, v.PatientId, bc.ChargeCode, bc.Description 
                            FROM BillLines l 
                            JOIN Bills b 
                                ON l.BillId = b.BillId
                            JOIN Visits v ON b.VisitId = v.VisitId
                            JOIN BillingCharges bc ON l.ReferenceId = bc.ChargeId
                            WHERE v.PatientId = @PatientId";
            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lines.Add(new BillLine
                    {
                        LineId = Convert.ToInt32(reader["LineId"]),
                        BillId = Convert.ToInt32(reader["BillId"]),
                        ReferenceId = Convert.ToInt32(reader["ReferenceId"]),
                        Description = reader["Description"].ToString(),
                        Rate = Convert.ToDecimal(reader["Rate"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                        PatientId = Convert.ToInt32(reader["PatientId"])
                    });
                }

                return lines;
            }

        }

    }

}