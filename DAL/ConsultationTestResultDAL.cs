using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class ConsultationTestResultDAL : BaseDAL, IDAL<ConsultationTestResult>
    {
        private readonly DbHelper _db;

        public ConsultationTestResultDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(ConsultationTestResult result, SqlConnection conn, SqlTransaction? tran = null)
        {
            int consultationTestResultId = 0;

            string query = @"
                INSERT INTO ConsultationTestResults
                (ClinicId, ConsultationTestResultGroupId, TestResultId, TestResultGroupId, TestPanelId, TestPanelCode, 
                TestPanelName, TestParameterId, TestParameterCode, TestParameterName, PanelDisplayOrder, 
                ParameterDisplayOrder, NormalRange, NormalMin, NormalMax, TestMethod, DataType, Value, Unit, ResultFlag)
                VALUES
                (@ClinicId, @ConsultationTestResultGroupId, @TestResultId, @TestResultGroupId, @TestPanelId, @TestPanelCode, 
                @TestPanelName, @TestParameterId, @TestParameterCode, @TestParameterName, @PanelDisplayOrder, 
                @ParameterDisplayOrder, @NormalRange, @NormalMin, @NormalMax, @TestMethod, @DataType, @Value, @Unit, @ResultFlag);
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", result.ClinicId);
            cmd.Parameters.AddWithValue("@ConsultationTestResultGroupId", result.ConsultationTestResultGroupId);
            cmd.Parameters.AddWithValue("@TestResultId", result.TestResultId);
            cmd.Parameters.AddWithValue("@TestResultGroupId", result.TestResultGroupId);
            cmd.Parameters.AddWithValue("@TestPanelId", result.TestPanelId);
            cmd.Parameters.AddWithValue("@TestPanelCode", result.TestPanelCode);
            cmd.Parameters.AddWithValue("@TestPanelName", result.TestPanelName);
            cmd.Parameters.AddWithValue("@TestParameterId", result.TestParameterId);
            cmd.Parameters.AddWithValue("@TestParameterCode", result.TestParameterCode);
            cmd.Parameters.AddWithValue("@TestParameterName", result.TestParameterName);
            cmd.Parameters.AddWithValue("@PanelDisplayOrder", result.PanelDisplayOrder);
            cmd.Parameters.AddWithValue("@ParameterDisplayOrder", result.ParameterDisplayOrder);
            cmd.Parameters.AddWithValue("@NormalRange", result.NormalRange ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalMin", result.NormalMin ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalMax", result.NormalMax ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TestMethod", result.TestMethod ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DataType", result.DataType);
            cmd.Parameters.AddWithValue("@Value", result.Value ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", result.Unit ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ResultFlag", result.ResultFlag ?? (object)DBNull.Value);

            consultationTestResultId = Convert.ToInt32(cmd.ExecuteScalar());

            result.ConsultationTestResultId = consultationTestResultId;

            return consultationTestResultId;
        }

        public int Add(List<ConsultationTestResult> results, SqlConnection conn, SqlTransaction? tran = null)
        {
            int consultationTestResultId = 0;

            string query = @"
                INSERT INTO ConsultationTestResults
                (ClinicId, ConsultationTestResultGroupId, TestResultId, TestResultGroupId, TestPanelId, TestPanelCode, 
                TestPanelName, TestParameterId, TestParameterCode, TestParameterName, PanelDisplayOrder, 
                ParameterDisplayOrder, NormalRange, NormalMin, NormalMax, TestMethod, DataType, Value, Unit, ResultFlag)
                VALUES
                (@ClinicId, @ConsultationTestResultGroupId, @TestResultId, @TestResultGroupId, @TestPanelId, @TestPanelCode, 
                @TestPanelName, @TestParameterId, @TestParameterCode, @TestParameterName, @PanelDisplayOrder, 
                @ParameterDisplayOrder, @NormalRange, @NormalMin, @NormalMax, @TestMethod, @DataType, @Value, @Unit, @ResultFlag);
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            foreach (var result in results) 
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ClinicId", result.ClinicId);
                cmd.Parameters.AddWithValue("@ConsultationTestResultGroupId", result.ConsultationTestResultGroupId);
                cmd.Parameters.AddWithValue("@TestResultId", result.TestResultId);
                cmd.Parameters.AddWithValue("@TestResultGroupId", result.TestResultGroupId);
                cmd.Parameters.AddWithValue("@TestPanelId", result.TestPanelId);
                cmd.Parameters.AddWithValue("@TestPanelCode", result.TestPanelCode);
                cmd.Parameters.AddWithValue("@TestPanelName", result.TestPanelName);
                cmd.Parameters.AddWithValue("@TestParameterId", result.TestParameterId);
                cmd.Parameters.AddWithValue("@TestParameterCode", result.TestParameterCode);
                cmd.Parameters.AddWithValue("@TestParameterName", result.TestParameterName);
                cmd.Parameters.AddWithValue("@PanelDisplayOrder", result.PanelDisplayOrder);
                cmd.Parameters.AddWithValue("@ParameterDisplayOrder", result.ParameterDisplayOrder);
                cmd.Parameters.AddWithValue("@NormalRange", result.NormalRange ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NormalMin", result.NormalMin ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NormalMax", result.NormalMax ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TestMethod", result.TestMethod ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DataType", result.DataType);
                cmd.Parameters.AddWithValue("@Value", result.Value ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Unit", result.Unit ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ResultFlag", result.ResultFlag ?? (object)DBNull.Value);

                consultationTestResultId = Convert.ToInt32(cmd.ExecuteScalar());

                result.ConsultationTestResultId = consultationTestResultId;
            }

            return consultationTestResultId;
        }

        public bool Update(ConsultationTestResult model, SqlConnection conn, SqlTransaction? tran = null)
        {
            throw new NotImplementedException("Update of Test Results not to be used.");
        }

        public bool Delete(int consultationVitalId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            throw new NotImplementedException("Delete one Test Result at a time not to be used.");            
        }

        public ConsultationTestResult Get(int consultationTestResultId, int clinicId, 
                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            const string query = @"
                SELECT *
                FROM ConsultationTestResults
                WHERE ConsultationTestResultId = @ConsultationTestResultId
                AND ClinicId = @ClinicId";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ConsultationTestResultId", consultationTestResultId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null; 
        }

        public List<ConsultationTestResult> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationTestResult> list = [];

            const string query = @"
                SELECT *
                FROM ConsultationTestResults
                WHERE ClinicId = @ClinicId
                ORDER BY t.TestDate DESC,
                        t.PanelDisplayOrder,
                        t.ParameterDisplayOrder";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapReader(reader));
            }

            return list;
        }

        public List<ConsultationTestResult> ListByConsultation(int consultationId, int clinicId, SqlConnection conn, 
                                                                                                SqlTransaction? tran = null)
        {
            List<ConsultationTestResult> list = [];

            const string query = @"
                SELECT t.*
                FROM ConsultationTestResults t
                INNER JOIN ConsultationTestResultGroups g
                    ON t.ConsultationTestResultGroupId = g.ConsultationTestResultGroupId
                    AND t.ClinicId = g.ClinicId
                INNER JOIN ConsultationRows r
                    ON g.RowId = r.RowId
                INNER JOIN ConsultationSections s
                    ON r.SectionId = s.SectionId
                WHERE s.ConsultationId = @ConsultationId
                AND t.ClinicId = @ClinicId
                ORDER BY t.PanelDisplayOrder,
                        t.ParameterDisplayOrder";

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

        public List<ConsultationTestResultGroup> ListGroupByConsultation(int consultationId, int clinicId, SqlConnection conn, 
                                                                                                SqlTransaction? tran = null)
        {
            List<ConsultationTestResultGroup> list = [];

            const string query = @"
                SELECT g.*
                FROM ConsultationTestResultGroups g
                INNER JOIN ConsultationRows r
                    ON g.RowId = r.RowId
                INNER JOIN ConsultationSections s
                    ON r.SectionId = s.SectionId
                WHERE s.ConsultationId = @ConsultationId
                AND g.ClinicId = @ClinicId
                ORDER BY g.TestDate DESC";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapGroupReader(reader));
            }

            return list;            
        }

        public List<ConsultationTestResult> ListByRow(int rowId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationTestResult> list = [];

            const string query = @"
                SELECT *
                FROM ConsultationTestResults t
                INNER JOIN ConsultationTestResultGroups g
                    ON t.ConsultationTestResultGroupId = g.ConsultationTestResultGroupId
                    AND t.ClinicId = g.ClinicId
                WHERE g.RowId = @RowId
                AND t.ClinicId = @ClinicId
                ORDER BY t.TestDate DESC,
                        t.PanelDisplayOrder,
                        t.ParameterDisplayOrder;";

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
                        DELETE tr
                        FROM ConsultationTestResults tr
                        INNER JOIN ConsultationTestResultGroups trg
                            ON trg.ConsultationTestResultGroupId = tr.ConsultationTestResultGroupId
                            AND trg.ClinicId = tr.ClinicId
                        WHERE trg.RowId = @RowId
                        AND tr.ClinicId = @ClinicId;";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@RowId", rowId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;            
        }

        public int AddResultGroups(List<ConsultationTestResultGroup> groups, SqlConnection conn, SqlTransaction? tran = null)
        {
            int consultationTestResultGroupId = 0;

            string query = @"
                INSERT INTO ConsultationTestResultGroups
                (ClinicId, PatientId, VisitId, RowId, TestDate)
                VALUES
                (@ClinicId, @PatientId, @VisitId, @RowId, @TestDate);
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            foreach (var group in groups) 
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ClinicId", group.ClinicId);
                cmd.Parameters.AddWithValue("@PatientId", group.PatientId);
                cmd.Parameters.AddWithValue("@VisitId", group.VisitId);
                cmd.Parameters.AddWithValue("@RowId", group.RowId);
                cmd.Parameters.AddWithValue("@TestDate", group.TestDate);

                consultationTestResultGroupId = Convert.ToInt32(cmd.ExecuteScalar());

                group.ConsultationTestResultGroupId = consultationTestResultGroupId;
            }

            return consultationTestResultGroupId;
        }

        public List<ConsultationTestResultGroup> ListGroupsByConsultation(int consultationId, int clinicId, SqlConnection conn, 
                                                                                                SqlTransaction? tran = null)
        {
            List<ConsultationTestResultGroup> list = [];

            const string query = @"
                SELECT g.*
                FROM ConsultationTestResultGroups g
                INNER JOIN ConsultationRows r
                    ON g.RowId = r.RowId
                INNER JOIN ConsultationSections s
                    ON r.SectionId = s.SectionId
                WHERE s.ConsultationId = @ConsultationId
                AND g.ClinicId = @ClinicId
                ORDER BY g.TestDate DESC";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapGroupReader(reader));
            }

            return list;            
        }

        public List<ConsultationTestResultGroup> ListGroupsByRow(int rowId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationTestResultGroup> list = [];

            const string query = @"
                SELECT *
                FROM ConsultationTestResultGroups g
                WHERE g.RowId = @RowId
                AND g.ClinicId = @ClinicId
                ORDER BY g.TestDate DESC;";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@RowId", rowId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapGroupReader(reader));
            }

            return list;            
        }

        public bool DeleteGroupsByRow(int rowId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            const string query = @"
                        DELETE
                        FROM ConsultationTestResultGroups
                        WHERE RowId = @RowId
                        AND ClinicId = @ClinicId;";

            using var cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@RowId", rowId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;            
        }

        private ConsultationTestResult MapReader(SqlDataReader reader)
        {
            return new ConsultationTestResult
            {
                ConsultationTestResultId = Convert.ToInt32(reader["ConsultationTestResultId"]),
                ConsultationTestResultGroupId = Convert.ToInt32(reader["ConsultationTestResultGroupId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                TestResultId = Convert.ToInt32(reader["TestResultId"]),
                TestResultGroupId = Convert.ToInt32(reader["TestResultGroupId"]),
                TestPanelId = Convert.ToInt32(reader["TestPanelId"]),
                TestPanelCode = Convert.ToString(reader["TestPanelCode"]) ?? "",
                TestPanelName = Convert.ToString(reader["TestPanelName"]) ?? "",
                TestParameterId = Convert.ToInt32(reader["TestParameterId"]),
                TestParameterCode = Convert.ToString(reader["TestParameterCode"]) ?? "",
                TestParameterName = Convert.ToString(reader["TestParameterName"]) ?? "",
                PanelDisplayOrder = Convert.ToInt32(reader["PanelDisplayOrder"]),
                ParameterDisplayOrder = Convert.ToInt32(reader["ParameterDisplayOrder"]),
                NormalRange = reader["NormalRange"] == DBNull.Value ? null : Convert.ToString(reader["NormalRange"]),
                NormalMin = reader["NormalMin"] == DBNull.Value ? null : Convert.ToDecimal(reader["NormalMin"]),
                NormalMax = reader["NormalMax"] == DBNull.Value ? null : Convert.ToDecimal(reader["NormalMax"]),
                TestMethod = reader["TestMethod"] == DBNull.Value ? null : Convert.ToString(reader["TestMethod"]),
                DataType = Convert.ToString(reader["DataType"]) ?? "",
                Value = reader["Value"] == DBNull.Value ? null : Convert.ToString(reader["Value"]),
                Unit = reader["Unit"] == DBNull.Value ? null : Convert.ToString(reader["Unit"]),
                ResultFlag = reader["ResultFlag"] == DBNull.Value ? null : Convert.ToString(reader["ResultFlag"])
            };
        }

        private ConsultationTestResultGroup MapGroupReader(SqlDataReader reader)
        {
            return new ConsultationTestResultGroup
            {
                ConsultationTestResultGroupId = Convert.ToInt32(reader["ConsultationTestResultGroupId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                PatientId = Convert.ToInt32(reader["PatientId"]),
                VisitId = Convert.ToInt32(reader["VisitId"]),
                RowId = Convert.ToInt32(reader["RowId"]),
                TestDate = Convert.ToDateTime(reader["TestDate"])
            };
        }

    }

}

