using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class RefundLineDAL : BaseDAL, IDAL<RefundLine>
    {
        private readonly DbHelper _db;

        public RefundLineDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(RefundLine line, SqlConnection conn, SqlTransaction tran)
        {
            var query = @"
                INSERT INTO RefundLines
                (RefundId, RefundLineNo, BillLineId, BillLineNo, ServiceType, Description, Quantity,
                    Rate, GrossAmount, DiscountPercent, DiscountAmount, TaxPercent, TaxAmount, NetAmount)
                VALUES
                (@RefundId, @RefundLineNo, @BillLineId, @BillLineNo, @ServiceType, @Description, @Quantity,
                    @Rate, @GrossAmount, @DiscountPercent, @DiscountAmount, @TaxPercent, @TaxAmount, @NetAmount);

                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundId", line.RefundId);
            cmd.Parameters.AddWithValue("@RefundLineNo", line.RefundLineNo);
            cmd.Parameters.AddWithValue("@BillLineId", line.BillLineId);
            cmd.Parameters.AddWithValue("@BillLineNo", line.BillLineNo);
            cmd.Parameters.AddWithValue("@ServiceType", line.ServiceType);
            cmd.Parameters.AddWithValue("@Description", line.Description ?? "");
            cmd.Parameters.AddWithValue("@Quantity", line.Quantity);
            cmd.Parameters.AddWithValue("@Rate", line.Rate);
            cmd.Parameters.AddWithValue("@GrossAmount", line.GrossAmount);
            cmd.Parameters.AddWithValue("@DiscountPercent", line.DiscountPercent);
            cmd.Parameters.AddWithValue("@DiscountAmount", line.DiscountAmount);
            cmd.Parameters.AddWithValue("@TaxPercent", line.TaxPercent);
            cmd.Parameters.AddWithValue("@TaxAmount", line.TaxAmount);
            cmd.Parameters.AddWithValue("@NetAmount", line.NetAmount);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int AddLines(List<RefundLine> lines, SqlConnection conn, SqlTransaction tran)
        {
            int lineId = 0;

            var query = @"
                INSERT INTO RefundLines
                (RefundId, RefundLineNo, BillLineId, BillLineNo, ServiceType, Description, Quantity,
                    Rate, GrossAmount, DiscountPercent, DiscountAmount, TaxPercent, TaxAmount, NetAmount)
                VALUES
                (@RefundId, @RefundLineNo, @BillLineId, @BillLineNo, @ServiceType, @Description, @Quantity,
                    @Rate, @GrossAmount, @DiscountPercent, @DiscountAmount, @TaxPercent, @TaxAmount, @NetAmount);

                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            foreach (var line in lines)
            {
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@RefundId", line.RefundId);
                cmd.Parameters.AddWithValue("@RefundLineNo", line.RefundLineNo);
                cmd.Parameters.AddWithValue("@BillLineId", line.BillLineId);
                cmd.Parameters.AddWithValue("@BillLineNo", line.BillLineNo);
                cmd.Parameters.AddWithValue("@ServiceType", line.ServiceType);
                cmd.Parameters.AddWithValue("@Description", line.Description ?? "");
                cmd.Parameters.AddWithValue("@Quantity", line.Quantity);
                cmd.Parameters.AddWithValue("@Rate", line.Rate);
                cmd.Parameters.AddWithValue("@GrossAmount", line.GrossAmount);
                cmd.Parameters.AddWithValue("@DiscountPercent", line.DiscountPercent);
                cmd.Parameters.AddWithValue("@DiscountAmount", line.DiscountAmount);
                cmd.Parameters.AddWithValue("@TaxPercent", line.TaxPercent);
                cmd.Parameters.AddWithValue("@TaxAmount", line.TaxAmount);
                cmd.Parameters.AddWithValue("@NetAmount", line.NetAmount);

                lineId = Convert.ToInt32(cmd.ExecuteScalar());
                line.RefundLineId = lineId;
            }

            return lineId;
        }

        public RefundLine Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var query = @"
                SELECT *
                FROM RefundLines
                WHERE RefundLineId = @RefundLineId";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@RefundLineId", id);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return MapRefundLine(reader);
                }
            }

            return new RefundLine();
        }

        public bool Update(RefundLine model, SqlConnection conn, SqlTransaction tran)
        {
            var query = @"
                UPDATE RefundLines
                SET
                    Quantity = @Quantity,
                    Rate = @Rate,
                    GrossAmount = @GrossAmount,
                    DiscountPercent = @DiscountPercent,
                    DiscountAmount = @DiscountAmount,
                    TaxPercent = @TaxPercent,
                    TaxAmount = @TaxAmount,
                    NetAmount = @NetAmount
                WHERE RefundLineId = @RefundLineId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
            cmd.Parameters.AddWithValue("@Rate", model.Rate);
            cmd.Parameters.AddWithValue("@GrossAmount", model.GrossAmount);
            cmd.Parameters.AddWithValue("@DiscountPercent", model.DiscountPercent);
            cmd.Parameters.AddWithValue("@DiscountAmount", model.DiscountAmount);
            cmd.Parameters.AddWithValue("@TaxPercent", model.TaxPercent);
            cmd.Parameters.AddWithValue("@TaxAmount", model.TaxAmount);
            cmd.Parameters.AddWithValue("@NetAmount", model.NetAmount);
            cmd.Parameters.AddWithValue("@RefundLineId", model.RefundLineId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            var query = @"DELETE FROM RefundLines WHERE RefundLineId = @RefundLineId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundLineId", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteLines(int refundId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            var query = @"DELETE FROM RefundLines WHERE RefundId = @RefundId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@RefundId", refundId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public List<RefundLine> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<RefundLine> lines = new List<RefundLine>();

            var query = @"
                SELECT rl.*
                FROM RefundLines rl
                INNER JOIN Refunds r
                    ON r.RefundId = rl.RefundId
                WHERE r.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lines.Add(MapRefundLine(reader));
                }
            }

            return lines;
        }

        public List<RefundLine> GetByRefundId(int refundId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<RefundLine> lines = new List<RefundLine>();

            var query = @"
                SELECT *
                FROM RefundLines
                WHERE RefundId = @RefundId
                ORDER BY RefundLineNo";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@RefundId", refundId);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lines.Add(MapRefundLine(reader));
                }
            }

            return lines;
        }

        private RefundLine MapRefundLine(SqlDataReader reader)
        {
            return new RefundLine
            {
                RefundLineId = Convert.ToInt32(reader["RefundLineId"]),
                RefundId = Convert.ToInt32(reader["RefundId"]),
                RefundLineNo = Convert.ToInt32(reader["RefundLineNo"]),
                BillLineId = Convert.ToInt32(reader["BillLineId"]),
                BillLineNo = Convert.ToInt32(reader["BillLineNo"]),
                ServiceType = (ServiceType)reader["ServiceType"],
                Description = GetNullableString(reader, "Description"),
                Quantity = Convert.ToInt32(reader["Quantity"]),
                Rate = Convert.ToDecimal(reader["Rate"]),
                DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                TaxPercent = Convert.ToDecimal(reader["TaxPercent"])
            };
        }
    }
}
