using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class VitalDAL : IDAL<Vital>
    {
        private readonly DbHelper _db;

        public VitalDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public int Add(Vital model, SqlConnection conn, SqlTransaction tran)
        {
            int vitalId = 0;
                    
            string query = @"
                INSERT INTO Vitals
                (VitalDefId, 
                    ClinicId,
                    VisitId,
                    VitalLabel,
                    VitalCode,
                    VitalType,
                    DataType,
                    VitalUnit,
                    VitalValue,
                    DeriveFormula,
                    DisplayComposition,
                    DisplayOrder)
                VALUES
                (@VitalDefId,
                    @ClinicId,
                    @VisitId,
                    @VitalLabel,
                    @VitalCode,
                    @VitalType,
                    @DataType,
                    @VitalUnit,
                    @VitalValue,
                    @DeriveFormula,
                    @DisplayComposition,
                    @DisplayOrder);

                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@VitalDefId", model.VitalDefId);
            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@VisitId", model.VisitId);
            cmd.Parameters.AddWithValue("@VitalLabel", model.VitalLabel ?? string.Empty);
            cmd.Parameters.AddWithValue("@VitalCode", model.VitalCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@VitalType", model.VitalType ?? string.Empty);
            cmd.Parameters.AddWithValue("@DataType", model.DataType ?? string.Empty);
            cmd.Parameters.AddWithValue("@VitalUnit", string.IsNullOrWhiteSpace(model.VitalUnit) ? DBNull.Value : model.VitalUnit);
            cmd.Parameters.AddWithValue("@VitalValue", string.IsNullOrWhiteSpace(model.VitalValue) ? DBNull.Value : model.VitalValue);
            cmd.Parameters.AddWithValue("@DeriveFormula", string.IsNullOrWhiteSpace(model.DeriveFormula) ? DBNull.Value : model.DeriveFormula);
            cmd.Parameters.AddWithValue("@DisplayComposition", string.IsNullOrWhiteSpace(model.DisplayComposition) ? DBNull.Value : model.DisplayComposition);
            cmd.Parameters.AddWithValue("@DisplayOrder", model.DisplayOrder == null ? DBNull.Value : model.DisplayOrder);

            vitalId = Convert.ToInt32(cmd.ExecuteScalar());
            return vitalId;
        }

        public bool Update(Vital model, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected;

            string query = @"
                UPDATE Vitals
                SET VitalValue = @VitalValue
                WHERE VitalId = @VitalId
                  AND ClinicId = @ClinicId
                  AND VisitId = @VisitId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@VitalValue", string.IsNullOrWhiteSpace(model.VitalValue) ? DBNull.Value : model.VitalValue);
            cmd.Parameters.AddWithValue("@VitalId", model.VitalId);
            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@VisitId", model.VisitId);

            rowsAffected = cmd.ExecuteNonQuery();
            
            return rowsAffected > 0;
        }

        public bool Delete(int vitalId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected;

            string query = @"
                DELETE Vitals
                WHERE VitalId = @VitalId
                  AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@VitalId", vitalId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            rowsAffected = cmd.ExecuteNonQuery();
            
            return rowsAffected > 0;
        }

        public Vital Get(int vitalId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT
                    VitalId,
                    VitalDefId,
                    ClinicId,
                    VisitId,
                    VitalLabel,
                    VitalCode,
                    VitalType,
                    DataType,
                    VitalUnit,
                    VitalValue,
                    DeriveFormula,
                    DisplayComposition,
                    DisplayOrder
                FROM Vitals
                WHERE VitalId = @VitalId
                  AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@VitalId", vitalId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapVital(reader);
        }

        public List<Vital> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            throw new NotImplementedException();
        }

        public List<Vital> GetAll(int clinicId, int visitId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Vital>();

            string query = @"
                SELECT VitalId, VitalDefId, ClinicId, VisitId, VitalLabel, VitalCode, VitalType, DataType, VitalUnit,
                        VitalValue, DeriveFormula, DisplayComposition, DisplayOrder
                FROM Vitals
                WHERE ClinicId = @ClinicId
                  AND VisitId = @VisitId
                ORDER BY DisplayOrder,
                        VitalDefId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@VisitId", visitId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapVital(reader));
            }

            return list;
        }

        public List<Vital> GetAll(int clinicId, int patientId, int visitId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Vital>();

            string query = @"
                SELECT vt.VitalId, vt.VitalDefId, vt.ClinicId, vt.VisitId, vt.VitalLabel, vt.VitalCode, vt.VitalType, 
                        vt.DataType, vt.VitalUnit, vt.VitalValue, vt.DeriveFormula, vt.DisplayComposition, 
                        vt.DisplayOrder
                FROM Vitals vt
                INNER JOIN Visits vs
                    ON vt.VisitId = vs.VisitId
                WHERE vt.ClinicId = @ClinicId
                    AND vs.PatientId = @PatientId
                    AND vt.VisitId = @VisitId
                ORDER BY DisplayOrder,
                        VitalDefId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@VisitId", visitId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapVital(reader));
            }

            return list;
        }

        private Vital MapVital(SqlDataReader reader)
        {
            return new Vital
            {
                VitalId = Convert.ToInt32(reader["VitalId"]),
                VitalDefId = Convert.ToInt32(reader["VitalDefId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                VisitId = Convert.ToInt32(reader["VisitId"]),
                VitalLabel = reader["VitalLabel"]?.ToString() ?? string.Empty,
                VitalCode = reader["VitalCode"]?.ToString() ?? string.Empty,
                VitalType = reader["VitalType"]?.ToString() ?? string.Empty,
                DataType = reader["DataType"]?.ToString() ?? string.Empty,
                VitalUnit = reader["VitalUnit"] != DBNull.Value ? reader["VitalUnit"].ToString() : null,
                VitalValue = reader["VitalValue"] != DBNull.Value ? reader["VitalValue"].ToString() : string.Empty,
                DeriveFormula = reader["DeriveFormula"] != DBNull.Value ? reader["DeriveFormula"].ToString() : string.Empty,
                DisplayComposition = reader["DisplayComposition"] != DBNull.Value ? reader["DisplayComposition"].ToString() : string.Empty,
                DisplayOrder = reader["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(reader["DisplayOrder"]) : null
            };
        }
    }

}