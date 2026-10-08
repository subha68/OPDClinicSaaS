using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class BillLineDAL : BaseDAL, IDAL<BillLine>
    {
        private readonly DbHelper _db;

        public BillLineDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(BillLine line, SqlConnection conn, SqlTransaction tran)
        {
            var query = @"INSERT INTO BillLines (BillId, ReferenceId, ReferenceCode, ReferenceName, Description, Rate, Quantity, 
                            GrossAmount, DiscountPercent, DiscountAmount, NetAmount, ServiceType, AvailableQuantity)
                            VALUES (@BillId, @ReferenceId, @ReferenceCode, @ReferenceName, @Description, @Rate, @Quantity, 
                            @GrossAmount, @DiscountPercent, @DiscountAmount, @NetAmount, @ServiceType, @AvailableQuantity);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@BillId", line.BillId);
            cmd.Parameters.AddWithValue("@ReferenceId", line.ReferenceId);
            cmd.Parameters.AddWithValue("@ReferenceCode", line.ReferenceCode);
            cmd.Parameters.AddWithValue("@ReferenceName", line.ReferenceName ?? line.ReferenceCode);
            cmd.Parameters.AddWithValue("@Description", line.Description);
            cmd.Parameters.AddWithValue("@Rate", line.Rate);
            cmd.Parameters.AddWithValue("@Quantity", line.Quantity);
            cmd.Parameters.AddWithValue("@GrossAmount", line.GrossAmount);
            cmd.Parameters.AddWithValue("@DiscountPercent", line.DiscountPercent);
            cmd.Parameters.AddWithValue("@DiscountAmount", line.DiscountAmount);
            cmd.Parameters.AddWithValue("@NetAmount", line.NetAmount);
            cmd.Parameters.AddWithValue("@ServiceType", line.ServiceType);
            cmd.Parameters.AddWithValue("@AvailableQuantity", line.AvailableQuantity);
            return Convert.ToInt32(Convert.ToInt32(cmd.ExecuteScalar()));
        }

        public BillLine Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var query = @"SELECT l.*, v.PatientId, bc.ChargeCode, bc.ChargeName
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
                        ReferenceCode = reader["ChargeCode"].ToString(),
                        ReferenceName = reader["ChargeName"].ToString(),
                        Description = reader["Description"].ToString(),
                        ServiceType = (ServiceType)reader["ChargeName"],
                        AvailableQuantity = Convert.ToInt32(reader["AvailableQuantity"])
                    };
                }
            }

            return null;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            var query = "DELETE FROM BillLines WHERE LineId = @LineId";
            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@LineId", id);

            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Update(BillLine model, SqlConnection conn, SqlTransaction tran)
        {
            var query = @"UPDATE BillLines 
                            SET BillId = @BillId, 
                                ReferenceId = @ReferenceId,
                                ReferenceCode = @ReferenceCode,
                                ReferenceName = @ReferenceName,
                                Description = @Description,
                                Rate = @Rate, 
                                Quantity = @Quantity, 
                                AvailableQuantity = @AvailableQuantity, 
                                DiscountPercent = @DiscountPercent
                                WHERE LineId = @LineId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@BillId", model.BillId);
            cmd.Parameters.AddWithValue("@ReferenceId", model.ReferenceId);
            cmd.Parameters.AddWithValue("@ReferenceCode", model.ReferenceCode);
            cmd.Parameters.AddWithValue("@ReferenceName", model.ReferenceName ?? model.ReferenceCode);
            cmd.Parameters.AddWithValue("@Description", model.Description);
            cmd.Parameters.AddWithValue("@Rate", model.Rate);
            cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
            cmd.Parameters.AddWithValue("@AvailableQuantity", model.AvailableQuantity);
            cmd.Parameters.AddWithValue("@DiscountPercent", model.DiscountPercent);
            cmd.Parameters.AddWithValue("@LineId", model.LineId);

            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateAvailableQuantities(List<BillLine> lines, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            var query = @"UPDATE BillLines SET 
                            AvailableQuantity = AvailableQuantity - @AvailableQuantity
                            WHERE LineId = @LineId
                            AND BillId = @BillId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            foreach (var line in lines)
            {
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@BillId", line.BillId);
                cmd.Parameters.AddWithValue("@AvailableQuantity", line.AvailableQuantity);
                cmd.Parameters.AddWithValue("@LineId", line.LineId);

                rowsAffected += cmd.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public List<BillLine> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
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
                        ReferenceCode = reader["ReferenceCode"].ToString(),
                        ReferenceName = reader["ReferenceName"].ToString(),
                        Description = reader["Description"].ToString(),
                        Rate = Convert.ToDecimal(reader["Rate"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                        PatientId = Convert.ToInt32(reader["PatientId"]),
                        ServiceType = (ServiceType)reader["ServiceType"],
                        AvailableQuantity = Convert.ToInt32(reader["AvailableQuantity"])
                    });
                }
            }

            return lines;
        }

        public List<BillLine> GetByBillId(int billId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<BillLine> lines = new List<BillLine>();

            var query = @"SELECT l.*, bc.ChargeCode, bc.ChargeName, v.PatientId 
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
                        ReferenceCode = reader["ReferenceCode"].ToString(),
                        ReferenceName = reader["ReferenceName"].ToString(), 
                        Description = reader["Description"].ToString(),
                        Rate = Convert.ToDecimal(reader["Rate"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                        PatientId = Convert.ToInt32(reader["PatientId"]),
                        ServiceType = (ServiceType)reader["ServiceType"],
                        AvailableQuantity = Convert.ToInt32(reader["AvailableQuantity"])
                    });
                }
            }

            return lines;
        }

        public List<BillLine> ListRefundableBillLines(int clinicId, int billId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<BillLine> lines = new List<BillLine>();

            var query = @"SELECT bl.*, bc.ChargeCode, bc.ChargeName, v.PatientId
                            FROM BillLines bl 
                            JOIN Bills b 
                                ON bl.BillId = b.BillId 
                            JOIN Visits v 
                                ON b.VisitId = v.VisitId
                            JOIN BillingCharges bc 
                                ON bl.ReferenceId = bc.ChargeId
                            WHERE b.BillId = @BillId
                            AND bl.AvailableQuantity > 0";

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
                        ReferenceCode = reader["ReferenceCode"].ToString(),
                        ReferenceName = reader["ReferenceName"].ToString(),
                        Description = reader["Description"].ToString(),
                        Rate = Convert.ToDecimal(reader["Rate"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                        PatientId = Convert.ToInt32(reader["PatientId"]),
                        ServiceType = (ServiceType)reader["ServiceType"],
                        AvailableQuantity = Convert.ToInt32(reader["AvailableQuantity"])
                    });
                }
            }

            return lines;
        }

        public List<BillLine> ListByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<BillLine> lines = new List<BillLine>();

            var query = @"SELECT l.*, v.PatientId, bc.ChargeCode, bc.ChargeName 
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
                        ReferenceCode = reader["ReferenceCode"].ToString(),
                        ReferenceName = reader["ReferenceName"].ToString(),
                        Description = reader["Description"].ToString(),
                        Rate = Convert.ToDecimal(reader["Rate"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                        PatientId = Convert.ToInt32(reader["PatientId"]),
                        ServiceType = (ServiceType)reader["ServiceType"],
                        AvailableQuantity = Convert.ToInt32(reader["AvailableQuantity"])
                    });
                }

                return lines;
            }

        }

    }

}