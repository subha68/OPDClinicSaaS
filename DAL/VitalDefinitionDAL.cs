using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Models;

namespace DAL
{
    public class VitalDefinitionDAL : IDAL<VitalDefinition>
    {
        private readonly DbHelper _db;

        public VitalDefinitionDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public int Add(VitalDefinition model, SqlConnection conn, SqlTransaction tran)
        {
            int vitalDefId = 0;

            string query = @"INSERT INTO VitalDefinitions 
                            (ClinicId, VitalLabel, VitalCode, VitalType, DataType, VitalUnit, IsDerived,
                            DeriveFormula, DisplayComposition, IsStandard, IsActive)
                            VALUES 
                            (@ClinicId, @VitalLabel, @VitalCode, @VitalType, @DataType, @VitalUnit, @IsDerived,
                            @DeriveFormula, @DisplayComposition, @IsStandard, @IsActive);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId.HasValue ? model.ClinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@VitalLabel", model.VitalLabel);
            cmd.Parameters.AddWithValue("@VitalCode", model.VitalCode);
            cmd.Parameters.AddWithValue("@VitalType", model.VitalType);
            cmd.Parameters.AddWithValue("@DataType", model.DataType);
            cmd.Parameters.AddWithValue("@VitalUnit", string.IsNullOrWhiteSpace(model.VitalUnit) ? DBNull.Value : model.VitalUnit);
            cmd.Parameters.AddWithValue("@IsDerived", model.IsDerived ? 1 : 0);
            cmd.Parameters.AddWithValue("@DeriveFormula", string.IsNullOrWhiteSpace(model.DeriveFormula) ? DBNull.Value : model.DeriveFormula);
            cmd.Parameters.AddWithValue("@DisplayComposition", string.IsNullOrWhiteSpace(model.DisplayComposition) ? DBNull.Value : model.DisplayComposition);
            cmd.Parameters.AddWithValue("@IsStandard", model.IsStandard ? 1 : 0);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive ? 1 : 0);

            vitalDefId = Convert.ToInt32(cmd.ExecuteScalar());

            return vitalDefId;
        }

        public bool Update(VitalDefinition model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE VitalDefinitions
                            SET ClinicId = @ClinicId,
                                VitalLabel = @VitalLabel,
                                VitalCode = @VitalCode,
                                VitalType = @VitalType,
                                DataType = @DataType,
                                VitalUnit = @VitalUnit,
                                IsDerived = @IsDerived,
                                DeriveFormula = @DeriveFormula,
                                DisplayComposition = @DisplayComposition,
                                IsStandard = @IsStandard,
                                IsActive = @IsActive
                            WHERE VitalDefId = @VitalDefId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId.HasValue ? model.ClinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@VitalLabel", model.VitalLabel);
            cmd.Parameters.AddWithValue("@VitalCode", model.VitalCode);
            cmd.Parameters.AddWithValue("@VitalType", model.VitalType);
            cmd.Parameters.AddWithValue("@DataType", model.DataType);
            cmd.Parameters.AddWithValue("@VitalUnit", string.IsNullOrWhiteSpace(model.VitalUnit) ? DBNull.Value : model.VitalUnit);
            cmd.Parameters.AddWithValue("@IsDerived", model.IsDerived ? 1 : 0);
            cmd.Parameters.AddWithValue("@DeriveFormula", string.IsNullOrWhiteSpace(model.DeriveFormula) ? DBNull.Value : model.DeriveFormula);
            cmd.Parameters.AddWithValue("@DisplayComposition", string.IsNullOrWhiteSpace(model.DisplayComposition) ? DBNull.Value : model.DisplayComposition);
            cmd.Parameters.AddWithValue("@IsStandard", model.IsStandard ? 1 : 0);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@VitalDefId", model.VitalDefId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int vitalDefId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM VitalDefinitions 
                                WHERE VitalDefId = @VitalDefId
                                AND ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@VitalDefId", vitalDefId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool DeleteDefault(int vitalDefId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM VitalDefinitions 
                                WHERE VitalDefId = @VitalDefId
                                AND ClinicId IS NULL";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@VitalDefId", vitalDefId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public VitalDefinition Get(int vitalDefId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT v.*, 
                                ISNULL(c.Name, '') as ClinicName 
                            FROM VitalDefinitions v
                            LEFT JOIN Clinics c 
                                ON v.ClinicId = c.ClinicId
                            WHERE v.VitalDefId = @VitalDefId
                            AND v.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@VitalDefId", vitalDefId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new VitalDefinition
                {
                    VitalDefId = (int)reader["VitalDefId"],
                    ClinicId = reader["ClinicId"] != DBNull.Value ? (int?)reader["ClinicId"] : null,
                    ClinicName = reader["ClinicName"] != DBNull.Value ? reader["ClinicName"].ToString() : null,
                    VitalLabel = reader["VitalLabel"].ToString(),
                    VitalCode = reader["VitalCode"].ToString(),
                    VitalType = reader["VitalType"].ToString(),
                    DataType = reader["DataType"].ToString(),
                    VitalUnit = reader["VitalUnit"] != DBNull.Value ? reader["VitalUnit"].ToString() : null,
                    IsDerived = Convert.ToBoolean(reader["IsDerived"]),
                    DeriveFormula = reader["DeriveFormula"] != DBNull.Value ? reader["DeriveFormula"].ToString() : null,
                    DisplayComposition = reader["DisplayComposition"] != DBNull.Value ? reader["DisplayComposition"].ToString() : null,
                    IsStandard = Convert.ToBoolean(reader["IsStandard"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
            }

            return null;
        }

        public VitalDefinition GetDefault(int vitalDefId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT v.*, 
                                '' as ClinicName 
                            FROM VitalDefinitions v
                            WHERE v.VitalDefId = @VitalDefId
                            AND v.ClinicId IS NULL";                

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@VitalDefId", vitalDefId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new VitalDefinition
                {
                    VitalDefId = (int)reader["VitalDefId"],
                    ClinicId = null,
                    ClinicName = reader["ClinicName"] != DBNull.Value ? reader["ClinicName"].ToString() : null,
                    VitalLabel = reader["VitalLabel"].ToString(),
                    VitalCode = reader["VitalCode"].ToString(),
                    VitalType = reader["VitalType"].ToString(),
                    DataType = reader["DataType"].ToString(),
                    VitalUnit = reader["VitalUnit"] != DBNull.Value ? reader["VitalUnit"].ToString() : null,
                    IsDerived = Convert.ToBoolean(reader["IsDerived"]),
                    DeriveFormula = reader["DeriveFormula"] != DBNull.Value ? reader["DeriveFormula"].ToString() : null,
                    DisplayComposition = reader["DisplayComposition"] != DBNull.Value ? reader["DisplayComposition"].ToString() : null,
                    IsStandard = Convert.ToBoolean(reader["IsStandard"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
            }

            return null;
        }

        public List<VitalDefinition> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<VitalDefinition>();

            string query = @"SELECT v.*, 
                                ISNULL(c.Name, '') as ClinicName 
                            FROM VitalDefinitions v
                            LEFT JOIN Clinics c 
                                ON v.ClinicId = c.ClinicId
                            WHERE v.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new VitalDefinition
                {
                    VitalDefId = (int)reader["VitalDefId"],
                    ClinicId = null,
                    ClinicName = reader["ClinicName"] != DBNull.Value ? reader["ClinicName"].ToString() : null,
                    VitalLabel = reader["VitalLabel"].ToString(),
                    VitalCode = reader["VitalCode"].ToString(),
                    VitalType = reader["VitalType"].ToString(),
                    DataType = reader["DataType"].ToString(),
                    VitalUnit = reader["VitalUnit"] != DBNull.Value ? reader["VitalUnit"].ToString() : null,
                    IsDerived = Convert.ToBoolean(reader["IsDerived"]),
                    DeriveFormula = reader["DeriveFormula"] != DBNull.Value ? reader["DeriveFormula"].ToString() : null,
                    DisplayComposition = reader["DisplayComposition"] != DBNull.Value ? reader["DisplayComposition"].ToString() : null,
                    IsStandard = Convert.ToBoolean(reader["IsStandard"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }

            return list;
        }

        public List<VitalDefinition> GetAllDefault(SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<VitalDefinition>();

            string query = @"SELECT v.*, '' as ClinicName 
                            FROM VitalDefinitions v
                            WHERE v.ClinicId IS NULL";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new VitalDefinition
                {
                    VitalDefId = (int)reader["VitalDefId"],
                    ClinicId = null,
                    ClinicName = reader["ClinicName"] != DBNull.Value ? reader["ClinicName"].ToString() : null,
                    VitalLabel = reader["VitalLabel"].ToString(),
                    VitalCode = reader["VitalCode"].ToString(),
                    VitalType = reader["VitalType"].ToString(),
                    DataType = reader["DataType"].ToString(),
                    VitalUnit = reader["VitalUnit"] != DBNull.Value ? reader["VitalUnit"].ToString() : null,
                    IsDerived = Convert.ToBoolean(reader["IsDerived"]),
                    DeriveFormula = reader["DeriveFormula"] != DBNull.Value ? reader["DeriveFormula"].ToString() : null,
                    DisplayComposition = reader["DisplayComposition"] != DBNull.Value ? reader["DisplayComposition"].ToString() : null,
                    IsStandard = Convert.ToBoolean(reader["IsStandard"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }

            return list;
        }
    }
}