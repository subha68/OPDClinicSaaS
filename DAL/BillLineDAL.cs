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

        public int Add(BillLine line)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                var query = @"INSERT INTO BillLines (BillId, ChargeId, Rate, Quantity, GrossAmount, DiscountPercent, DiscountAmount, NetAmount)
                            VALUES (@BillId, @ChargeId, @Rate, @Quantity, @GrossAmount, @DiscountPercent, @DiscountAmount, @NetAmount);
                            SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@BillId", line.BillId);
                cmd.Parameters.AddWithValue("@ChargeId", line.BillingChargeId);
                cmd.Parameters.AddWithValue("@Rate", line.Rate);
                cmd.Parameters.AddWithValue("@Quantity", line.Quantity);
                cmd.Parameters.AddWithValue("@GrossAmount", line.GrossAmount);
                cmd.Parameters.AddWithValue("@DiscountPercent", line.DiscountPercent);
                cmd.Parameters.AddWithValue("@DiscountAmount", line.DiscountAmount);
                cmd.Parameters.AddWithValue("@NetAmount", line.NetAmount);

                return Convert.ToInt32(Convert.ToInt32(cmd.ExecuteScalar()));
            }
        }

        public BillLine Get(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                var query = "SELECT * FROM BillLines WHERE LineId = @LineId";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@LineId", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BillLine
                        {
                            LineId = Convert.ToInt32(reader["LineId"]),
                            BillId = Convert.ToInt32(reader["BillId"]),
                            BillingChargeId = Convert.ToInt32(reader["ChargeId"]),
                            Rate = Convert.ToDecimal(reader["Rate"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"])
                        };
                    }
                }
            }
            return null;
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                var query = "DELETE FROM BillLines WHERE LineId = @LineId";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@LineId", id);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public bool Update(BillLine model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                var query = @"UPDATE BillLines SET 
                            BillId = @BillId, 
                            ChargeId = @ChargeId, 
                            Rate = @Rate, 
                            Quantity = @Quantity, 
                            DiscountPercent = @DiscountPercent
                            WHERE LineId = @LineId";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@BillId", model.BillId);
                cmd.Parameters.AddWithValue("@ChargeId", model.BillingChargeId);
                cmd.Parameters.AddWithValue("@Rate", model.Rate);
                cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                cmd.Parameters.AddWithValue("@DiscountPercent", model.DiscountPercent);
                cmd.Parameters.AddWithValue("@LineId", model.LineId);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public List<BillLine> GetAll()
        {
            List<BillLine> lines = new List<BillLine>();

            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                var query = "SELECT * FROM BillLines";
                SqlCommand cmd = new SqlCommand(query, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lines.Add(new BillLine
                        {
                            LineId = Convert.ToInt32(reader["LineId"]),
                            BillId = Convert.ToInt32(reader["BillId"]),
                            BillingChargeId = Convert.ToInt32(reader["BillingChargeId"]),
                            Rate = Convert.ToDecimal(reader["Rate"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"])
                        });
                    }
                }
            }

            return lines;
        }

        public List<BillLine> GetByBillId(int billId)
        {
            List<BillLine> lines = new List<BillLine>();

            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                var query = "SELECT * FROM BillLines WHERE BillId = @BillId";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@BillId", billId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lines.Add(new BillLine
                        {
                            LineId = Convert.ToInt32(reader["LineId"]),
                            BillId = Convert.ToInt32(reader["BillId"]),
                            BillingChargeId = Convert.ToInt32(reader["ChargeId"]),
                            Rate = Convert.ToDecimal(reader["Rate"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"])
                        });
                    }
                }
            }

            return lines;
        }

    }

}