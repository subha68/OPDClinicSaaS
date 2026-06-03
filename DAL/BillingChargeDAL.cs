using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class BillingChargeDAL : IDAL<BillingCharge>
    {
        private readonly DbHelper _db;

        public BillingChargeDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(BillingCharge charge, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO BillingCharges (ClinicId, ChargeCode, ChargeName, Description, Rate, IsActive)
                                VALUES (@clinicId, @code, @name, @desc, @rate, @active);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@clinicId", charge.ClinicId);
            cmd.Parameters.AddWithValue("@code", charge.ChargeCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", charge.ChargeName);
            cmd.Parameters.AddWithValue("@desc", charge.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@rate", charge.Rate);
            cmd.Parameters.AddWithValue("@active", charge.IsActive);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(BillingCharge charge, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE BillingCharges
                                 SET ChargeCode = @code,
                                     ChargeName = @name,
                                     Description = @desc,
                                     Rate = @rate,
                                     IsActive = @active
                                 WHERE ChargeId = @id AND ClinicId = @clinicId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@code", charge.ChargeCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", charge.ChargeName);
            cmd.Parameters.AddWithValue("@desc", charge.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@rate", charge.Rate);
            cmd.Parameters.AddWithValue("@active", charge.IsActive);
            cmd.Parameters.AddWithValue("@id", charge.ChargeId);
            cmd.Parameters.AddWithValue("@clinicId", charge.ClinicId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int chargeId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM BillingCharges WHERE ChargeId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", chargeId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public BillingCharge Get(int chargeId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT ChargeId, ClinicId, ChargeCode, ChargeName, Description, Rate, IsActive
                                    FROM BillingCharges
                                    WHERE ChargeId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", chargeId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BillingCharge
                        {
                            ChargeId = reader.GetInt32(0),
                            ClinicId = reader.GetInt32(1),
                            ChargeCode = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ChargeName = reader.GetString(3),
                            Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Rate = reader.GetDecimal(5),
                            IsActive = reader.GetBoolean(6)
                        };
                    }
                }
            }

            return null;
        }

        public List<BillingCharge> GetAll()
        {
            var list = new List<BillingCharge>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT ChargeId, ClinicId, ChargeCode, ChargeName, Description, Rate, IsActive FROM BillingCharges", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new BillingCharge
                    {
                        ChargeId = reader.GetInt32(0),
                        ClinicId = reader.GetInt32(1),
                        ChargeCode = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ChargeName = reader.GetString(3),
                        Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Rate = reader.GetDecimal(5),
                        IsActive = reader.GetBoolean(6)
                    });
                }
            }

            return list;
        }

        public List<BillingCharge> GetAllByClinic(int clinicId)
        {
            var list = new List<BillingCharge>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand(@"SELECT ChargeId, ClinicId, ChargeCode, ChargeName, Description, Rate, IsActive 
                                        FROM BillingCharges WHERE ClinicId = @clinicId", conn);
                cmd.Parameters.AddWithValue("@clinicId", clinicId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new BillingCharge
                    {
                        ChargeId = reader.GetInt32(0),
                        ClinicId = reader.GetInt32(1),
                        ChargeCode = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ChargeName = reader.GetString(3),
                        Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Rate = reader.GetDecimal(5),
                        IsActive = reader.GetBoolean(6)
                    });
                }
            }

            return list;
        }
    }
}