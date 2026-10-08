using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class BillingChargeDAL : BaseDAL, IDAL<BillingCharge>
    {
        private readonly DbHelper _db;

        public BillingChargeDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(BillingCharge charge, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO BillingCharges (ClinicId, ChargeCode, ChargeName, Description, ServiceType, Rate, IsActive)
                                VALUES (@clinicId, @code, @name, @desc, @serviceType, @rate, @active);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@clinicId", charge.ClinicId);
            cmd.Parameters.AddWithValue("@code", charge.ChargeCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", charge.ChargeName);
            cmd.Parameters.AddWithValue("@serviceType", charge.ServiceType);
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
                                     ServiceType = @serviceType,
                                     Rate = @rate,
                                     IsActive = @active
                                 WHERE ChargeId = @id AND ClinicId = @clinicId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@code", charge.ChargeCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", charge.ChargeName);
            cmd.Parameters.AddWithValue("@desc", charge.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@serviceType", charge.ServiceType);
            cmd.Parameters.AddWithValue("@rate", charge.Rate);
            cmd.Parameters.AddWithValue("@active", charge.IsActive);
            cmd.Parameters.AddWithValue("@id", charge.ChargeId);
            cmd.Parameters.AddWithValue("@clinicId", charge.ClinicId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int chargeId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM BillingCharges WHERE ChargeId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", chargeId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public BillingCharge Get(int chargeId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT ChargeId, ClinicId, ChargeCode, ChargeName, Description, ServiceType, Rate, IsActive
                                FROM BillingCharges
                                WHERE ChargeId = @id";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@id", chargeId);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new BillingCharge
                    {
                        ChargeId = Convert.ToInt32(reader["ChargeId"]),
                        ClinicId = Convert.ToInt32(reader["ClinicId"]),
                        ChargeCode = reader["ChargeCode"].ToString(),
                        ChargeName = reader["ChargeName"].ToString(),
                        Description = reader["Description"].ToString(),
                        ServiceType = (ServiceType)reader["ServiceType"],
                        Rate = Convert.ToDecimal(reader["Rate"]),
                        IsActive = (bool)reader["IsActive"]
                    };
                }
            }

            return null;
        }

        public List<BillingCharge> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<BillingCharge>();

            var cmd = new SqlCommand(@"SELECT ChargeId, ClinicId, ChargeCode, ChargeName, Description, 
                                        ServiceType, Rate, IsActive 
                                        FROM BillingCharges
                                        AND ChargeCode <> 'CONSULT'", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new BillingCharge
                {
                    ChargeId = Convert.ToInt32(reader["ChargeId"]),
                    ClinicId = Convert.ToInt32(reader["ClinicId"]),
                    ChargeCode = reader["ChargeCode"].ToString(),
                    ChargeName = reader["ChargeName"].ToString(),
                    Description = reader["Description"].ToString(),
                    ServiceType = (ServiceType)reader["ServiceType"],
                    Rate = Convert.ToDecimal(reader["Rate"]),
                    IsActive = (bool)reader["IsActive"]
                });
            }

            return list;
        }

        public List<BillingCharge> GetAllByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<BillingCharge>();

            var cmd = new SqlCommand(@"SELECT ChargeId, ClinicId, ChargeCode, ChargeName, Description, ServiceType, Rate, IsActive 
                                    FROM BillingCharges 
                                    WHERE ClinicId = @clinicId
                                    AND ChargeCode <> 'CONSULT'", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@clinicId", clinicId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new BillingCharge
                {
                    ChargeId = Convert.ToInt32(reader["ChargeId"]),
                    ClinicId = Convert.ToInt32(reader["ClinicId"]),
                    ChargeCode = reader["ChargeCode"].ToString(),
                    ChargeName = reader["ChargeName"].ToString(),
                    Description = reader["Description"].ToString(),
                    ServiceType = (ServiceType)reader["ServiceType"],
                    Rate = Convert.ToDecimal(reader["Rate"]),
                    IsActive = (bool)reader["IsActive"]
                });
            }

            return list;
        }

        public BillingCharge? GetByCode(string chargeCode, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var cmd = new SqlCommand(@"SELECT ChargeId, ClinicId, ChargeCode, ChargeName, Description, ServiceType, Rate, IsActive 
                                    FROM BillingCharges 
                                    WHERE ClinicId = @ClinicId
                                    AND ChargeCode = @ChargeCode", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@ChargeCode", chargeCode);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new BillingCharge
                {
                    ChargeId = Convert.ToInt32(reader["ChargeId"]),
                    ClinicId = Convert.ToInt32(reader["ClinicId"]),
                    ChargeCode = reader["ChargeCode"].ToString(),
                    ChargeName = reader["ChargeName"].ToString(),
                    Description = reader["Description"].ToString(),
                    ServiceType = (ServiceType)reader["ServiceType"],
                    Rate = Convert.ToDecimal(reader["Rate"]),
                    IsActive = (bool)reader["IsActive"]
                };
            }

            return null;
        }

        public List<BillingCharge> Search(int clinicId, string term, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<BillingCharge>();

            var cmd = new SqlCommand(@"SELECT ChargeId, 
                                                ClinicId, 
                                                ChargeCode, 
                                                ChargeName, 
                                                Description, 
                                                ServiceType, 
                                                Rate, 
                                                IsActive 
                                    FROM BillingCharges 
                                    WHERE (ChargeCode LIKE '%' + @term + '%'
                                    OR ChargeName LIKE '%' + @term + '%')
                                    AND ClinicId = @clinicId
                                    AND ChargeCode <> 'CONSULT'", conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@clinicId", clinicId);
            cmd.Parameters.AddWithValue("@term", term);
            
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new BillingCharge
                {
                    ChargeId = Convert.ToInt32(reader["ChargeId"]),
                    ClinicId = Convert.ToInt32(reader["ClinicId"]),
                    ChargeCode = reader["ChargeCode"].ToString(),
                    ChargeName = reader["ChargeName"].ToString(),
                    Description = reader["Description"].ToString(),
                    ServiceType = (ServiceType)reader["ServiceType"],
                    Rate = Convert.ToDecimal(reader["Rate"]),
                    IsActive = (bool)reader["IsActive"]
                });
            }

            return list;
        }

    }
}