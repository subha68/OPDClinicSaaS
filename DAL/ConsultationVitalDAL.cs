using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class ConsultationVitalDAL : BaseDAL, IDAL<ConsultationVital>
    {
        private readonly DbHelper _db;

        public ConsultationVitalDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(ConsultationVital vital, SqlConnection conn, SqlTransaction? tran = null)
        {
            int consultationVitalId = 0;

            string query = @"
                INSERT INTO ConsultationVitals
                (ClinicId, RowId, VitalId, VitalDefId, VisitId, VisitDate, VitalLabel, VitalCode, VitalType, 
                DataType, VitalUnit, VitalValue, DeriveFormula, DisplayComposition, DisplayOrder)
                VALUES
                (@ClinicId, @RowId, @VitalId, @VitalDefId, @VisitId, @VisitDate, @VitalLabel, @VitalCode, @VitalType,
                @DataType, @VitalUnit, @VitalValue, @IsDerived, @DeriveFormula, @DisplayComposition, @DisplayOrder);
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ClinicId", vital.ClinicId);
            cmd.Parameters.AddWithValue("@RowId", vital.RowId);
            cmd.Parameters.AddWithValue("@VitalId", vital.VitalId);
            cmd.Parameters.AddWithValue("@VitalDefId", vital.VitalDefId);
            cmd.Parameters.AddWithValue("@VisitId", vital.VisitId);
            cmd.Parameters.AddWithValue("@VisitDate", vital.VisitDate);
            cmd.Parameters.AddWithValue("@VitalLabel", vital.VitalLabel);
            cmd.Parameters.AddWithValue("@VitalCode", vital.VitalCode);
            cmd.Parameters.AddWithValue("@VitalType", vital.VitalType);
            cmd.Parameters.AddWithValue("@DataType", vital.DataType);
            cmd.Parameters.AddWithValue("@VitalUnit", vital.VitalUnit ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VitalValue", vital.VitalValue ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DeriveFormula", vital.DeriveFormula ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DisplayComposition", vital.DisplayComposition ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DisplayOrder", vital.DisplayOrder ?? (object)DBNull.Value);

            consultationVitalId = Convert.ToInt32(cmd.ExecuteScalar());

            vital.ConsultationVitalId = consultationVitalId;

            return consultationVitalId;
        }

        public int Add(List<ConsultationVital> vitals, SqlConnection conn, SqlTransaction? tran = null)
        {
            int consultationVitalId = 0;

            string query = @"
                INSERT INTO ConsultationVitals
                (ClinicId, RowId, VitalId, VitalDefId, VisitId, VisitDate, VitalLabel, VitalCode, VitalType, 
                DataType, VitalUnit, VitalValue, DeriveFormula, DisplayComposition, DisplayOrder)
                VALUES
                (@ClinicId, @RowId, @VitalId, @VitalDefId, @VisitId, @VisitDate, @VitalLabel, @VitalCode, @VitalType,
                @DataType, @VitalUnit, @VitalValue, @DeriveFormula, @DisplayComposition, @DisplayOrder);
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            foreach (var vital in vitals) 
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ClinicId", vital.ClinicId);
                cmd.Parameters.AddWithValue("@RowId", vital.RowId);
                cmd.Parameters.AddWithValue("@VitalId", vital.VitalId);
                cmd.Parameters.AddWithValue("@VitalDefId", vital.VitalDefId);
                cmd.Parameters.AddWithValue("@VisitId", vital.VisitId);
                cmd.Parameters.AddWithValue("@VisitDate", vital.VisitDate);
                cmd.Parameters.AddWithValue("@VitalLabel", vital.VitalLabel);
                cmd.Parameters.AddWithValue("@VitalCode", vital.VitalCode);
                cmd.Parameters.AddWithValue("@VitalType", vital.VitalType);
                cmd.Parameters.AddWithValue("@DataType", vital.DataType);
                cmd.Parameters.AddWithValue("@VitalUnit", vital.VitalUnit ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VitalValue", vital.VitalValue ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeriveFormula", vital.DeriveFormula ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DisplayComposition", vital.DisplayComposition ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DisplayOrder", vital.DisplayOrder ?? (object)DBNull.Value);

                consultationVitalId = Convert.ToInt32(cmd.ExecuteScalar());

                vital.ConsultationVitalId = consultationVitalId;
            }

            return consultationVitalId;
        }

        public bool Update(ConsultationVital model, SqlConnection conn, SqlTransaction? tran = null)
        {
            throw new NotImplementedException("Update of Vitals not to be used.");
        }

        public bool Delete(int consultationVitalId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            throw new NotImplementedException("Delete one Vital at a time not to be used.");            
        }

        public ConsultationVital Get(int consultationVitalId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            const string query = @"
                SELECT *
                FROM ConsultationVitals
                WHERE ConsultationVitalId = @ConsultationVitalId
                AND ClinicId = @ClinicId";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ConsultationVitalId", consultationVitalId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null; 
        }

        public List<ConsultationVital> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationVital> list = [];

            const string query = @"
                SELECT *
                FROM ConsultationVitals
                WHERE ClinicId = @ClinicId
                ORDER BY RowId, DisplayOrder";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapReader(reader));
            }

            return list;
        }

        public List<ConsultationVital> ListByConsultation(int consultationId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationVital> list = [];

            const string query = @"
                SELECT v.*
                FROM ConsultationVitals v
                INNER JOIN ConsultationRows r
                    ON v.RowId = r.RowId
                INNER JOIN ConsultationSections s
                    ON r.SectionId = s.SectionId
                WHERE s.ConsultationId = @ConsultationId
                AND v.ClinicId = @ClinicId
                ORDER BY v.DisplayOrder";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapReader(reader));
            }

            return list;            
        }

        public List<ConsultationVital> ListByRow(int rowId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationVital> list = [];

            const string query = @"
                SELECT *
                FROM ConsultationVitals
                WHERE RowId = @RowId
                AND ClinicId = @ClinicId
                ORDER BY DisplayOrder";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@RowId", rowId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapReader(reader));
            }

            return list;            
        }

        public bool DeleteByRow(int rowId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            const string query = @"
                DELETE
                FROM ConsultationVitals
                WHERE RowId = @RowId
                AND ClinicId = @ClinicId";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@RowId", rowId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;            
        }

        private ConsultationVital MapReader(SqlDataReader reader)
        {
            return new ConsultationVital
            {
                ConsultationVitalId = Convert.ToInt32(reader["ConsultationVitalId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                RowId = Convert.ToInt32(reader["RowId"]),
                VitalId = Convert.ToInt32(reader["VitalId"]),
                VitalDefId = Convert.ToInt32(reader["VitalDefId"]),
                VisitId = Convert.ToInt32(reader["VisitId"]),
                VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                VitalLabel = Convert.ToString(reader["VitalLabel"]) ?? "",
                VitalCode = Convert.ToString(reader["VitalCode"]) ?? "",
                VitalType = Convert.ToString(reader["VitalType"]) ?? "",
                DataType = Convert.ToString(reader["DataType"]) ?? "",
                VitalUnit = reader["VitalUnit"] as string,
                VitalValue = reader["VitalValue"] as string,
                DeriveFormula = reader["DeriveFormula"] as string,
                DisplayComposition = reader["DisplayComposition"] as string,
                DisplayOrder = reader["DisplayOrder"] == DBNull.Value ? null : Convert.ToInt32(reader["DisplayOrder"])
            };
        }       

    }

}

