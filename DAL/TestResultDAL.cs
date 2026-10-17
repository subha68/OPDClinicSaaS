using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class TestResultDAL : BaseDAL, IDAL<TestResult>
    {
        private readonly DbHelper _db;

        public TestResultDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public int Add(TestResult model, SqlConnection conn, SqlTransaction tran)
        {
            const string query = @"
                INSERT INTO TestResults
                (
                    TestResultGroupId, ClinicId, TestPanelId, TestPanelCode, TestPanelName, TestParameterId, TestParameterCode,
                    TestParameterName, PanelDisplayOrder, ParameterDisplayOrder, NormalRange, NormalMin, NormalMax,
                    TestMethod, DataType, Value, Unit, ResultFlag
                )
                VALUES
                (
                    @TestResultGroupId, @ClinicId, @TestPanelId, @TestPanelCode, @TestPanelName, @TestParameterId, 
                    @TestParameterCode, @TestParameterName, @PanelDisplayOrder, @ParameterDisplayOrder, @NormalRange, 
                    @NormalMin, @NormalMax, @TestMethod, @DataType, @Value, @Unit, @ResultFlag
                );

                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestResultGroupId", model.TestResultGroupId);
            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@TestPanelId", model.TestPanelId);
            cmd.Parameters.AddWithValue("@TestPanelCode", model.TestPanelCode);
            cmd.Parameters.AddWithValue("@TestPanelName", model.TestPanelName);
            cmd.Parameters.AddWithValue("@TestParameterId", model.TestParameterId);
            cmd.Parameters.AddWithValue("@TestParameterCode", model.TestParameterCode);
            cmd.Parameters.AddWithValue("@TestParameterName", model.TestParameterName);
            cmd.Parameters.AddWithValue("@PanelDisplayOrder", model.PanelDisplayOrder);
            cmd.Parameters.AddWithValue("@ParameterDisplayOrder", model.ParameterDisplayOrder);
            cmd.Parameters.AddWithValue("@NormalRange", (object?)model.NormalRange ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalMin", (object?)model.NormalMin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalMax", (object?)model.NormalMax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TestMethod", (object?)model.TestMethod ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DataType", model.DataType);
            cmd.Parameters.AddWithValue("@Value", (object?)model.Value ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", (object?)model.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ResultFlag", (object?)model.ResultFlag ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(TestResult model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE TestResults
                SET
                    Value = @Value,
                    ResultFlag = @ResultFlag,
                    Remarks = @Remarks
                WHERE TestResultId = @TestResultId
                AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestResultId", model.TestResultId);
            cmd.Parameters.AddWithValue("@Value", (object?)model.Value ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ResultFlag", model.ResultFlag);
            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestResults
                WHERE TestResultId = @TestResultId
                  AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestResultId", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteByResultGroup(int testResultGroupId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestResults
                WHERE TestResultGroupId = @TestResultGroupId
                  AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestResultGroupId", testResultGroupId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public TestResult? Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT R.TestResultId, R.TestResultGroupId, R.ClinicId, R.TestPanelId, R.TestPanelCode, R.TestPanelName,
                    R.TestParameterId, R.TestParameterCode, R.TestParameterName, R.PanelDisplayOrder, R.ParameterDisplayOrder,
                    R.NormalRange, R.NormalMin, R.NormalMax, R.TestMethod, R.DataType, R.Value, R.Unit,
                    R.ResultFlag, G.VisitId, G.VisitDate, G.TestDate
                FROM TestResults R
                INNER JOIN TestResultGroups G
                  ON R.TestResultGroupId = G.TestResultGroupId
                WHERE TestResultId = @TestResultId
                  AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestResultId", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapTestResult(reader);

            return null;
        }

        public List<TestResult> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResult>();

            string query = @"
                SELECT R.TestResultId, R.TestResultGroupId, R.ClinicId, R.TestPanelId, R.TestPanelCode, R.TestPanelName,
                    R.TestParameterId, R.TestParameterCode, R.TestParameterName, R.PanelDisplayOrder, R.ParameterDisplayOrder,
                    R.NormalRange, R.NormalMin, R.NormalMax, R.TestMethod, R.DataType, R.Value, R.Unit,
                    R.ResultFlag, G.VisitId, G.VisitDate, G.TestDate
                FROM TestResults R
                INNER JOIN TestResultGroups G
                  ON R.TestResultGroupId = G.TestResultGroupId
                WHERE ClinicId = @ClinicId
                ORDER BY
                    PanelDisplayOrder,
                    ParameterDisplayOrder,
                    TestResultId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResult(reader));
            }

            return list;
        }

        public List<TestResult> GetByResultGroup(int testResultGroupId, int clinicId, 
                                                    SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResult>();

            string query = @"
                SELECT R.TestResultId, R.TestResultGroupId, R.ClinicId, R.TestPanelId, R.TestPanelCode, R.TestPanelName,
                    R.TestParameterId, R.TestParameterCode, R.TestParameterName, R.PanelDisplayOrder, R.ParameterDisplayOrder,
                    R.NormalRange, R.NormalMin, R.NormalMax, R.TestMethod, R.DataType, R.Value, R.Unit,
                    R.ResultFlag, G.VisitId, V.VisitDate, G.TestDate
                FROM TestResults R
                INNER JOIN TestResultGroups G
                  ON R.TestResultGroupId = G.TestResultGroupId
                LEFT JOIN Visits V
                  ON G.VisitId = V.VisitId
                WHERE R.TestResultGroupId = @TestResultGroupId
                  AND R.ClinicId = @ClinicId
                ORDER BY
                    PanelDisplayOrder,
                    ParameterDisplayOrder,
                    TestResultId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestResultGroupId", testResultGroupId);

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResult(reader));
            }

            return list;
        }

        public int AddTestResultGroup(TestResultGroup model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO TestResultGroups
                (ClinicId, PatientId, VisitId, TestDate, ResultDate, LabName, ReportNo, Remarks
                )
                VALUES
                (@ClinicId, @PatientId, @VisitId, @TestDate, @ResultDate, @LabName, @ReportNo,
                    @Remarks
                );

                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@PatientId", model.PatientId);
            cmd.Parameters.AddWithValue("@VisitId", (object?)model.VisitId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TestDate", model.TestDate);
            cmd.Parameters.AddWithValue("@ResultDate", (object?)model.ResultDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LabName", (object?)model.LabName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReportNo", (object?)model.ReportNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool UpdateTestResultGroup(TestResultGroup model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE TestResultGroups
                SET
                    PatientId = @PatientId,
                    VisitId = @VisitId,
                    TestDate = @TestDate,
                    ResultDate = @ResultDate,
                    LabName = @LabName,
                    ReportNo = @ReportNo,
                    Remarks = @Remarks
                WHERE TestResultGroupId = @TestResultGroupId
                  AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestResultGroupId", model.TestResultGroupId);
            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@PatientId", model.PatientId);
            cmd.Parameters.AddWithValue("@VisitId", (object?)model.VisitId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TestDate", model.TestDate);
            cmd.Parameters.AddWithValue("@ResultDate", (object?)model.ResultDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LabName", (object?)model.LabName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReportNo", (object?)model.ReportNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteTestResultGroup(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestResultGroups
                WHERE TestResultGroupId = @TestResultGroupId
                  AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestResultGroupId", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public TestResultGroup? GetTestResultGroup(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT TestResultGroupId, ClinicId, PatientId, VisitId, TestDate,
                        ResultDate, LabName, ReportNo, Remarks, CreatedDate
                FROM TestResultGroups
                WHERE TestResultGroupId = @TestResultGroupId
                  AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestResultGroupId", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapTestResultGroup(reader);

            return null;
        }

        public List<TestResultGroup> GetGroupsByPatientAndDate(int patientId, DateTime testDate, int clinicId, 
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResultGroup>();

            string query = @"
                SELECT TestResultGroupId, ClinicId, PatientId, VisitId, TestDate,
                        ResultDate, LabName, ReportNo, Remarks, CreatedDate
                FROM TestResultGroups
                WHERE ClinicId = @ClinicId
                AND PatientId = @PatientId
                AND CAST(TestDate AS DATE) = @TestDate
                ORDER BY TestResultGroupId DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@TestDate", testDate.Date);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResultGroup(reader));
            }

            return list;
        }

        public List<TestResultGroup> GetAllTestResultGroups(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResultGroup>();

            string query = @"
                SELECT TestResultGroupId, ClinicId, PatientId, VisitId, TestDate,
                        ResultDate, LabName, ReportNo, Remarks, CreatedDate
                FROM TestResultGroups
                WHERE ClinicId = @ClinicId
                ORDER BY TestDate DESC, TestResultGroupId DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResultGroup(reader));
            }

            return list;
        }

        public List<TestResultGroup> GetResultGroupsByPatient(int patientId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResultGroup>();

            string query = @"
                SELECT TestResultGroupId, ClinicId, PatientId, VisitId, TestDate,
                        ResultDate, LabName, ReportNo, Remarks, CreatedDate
                FROM TestResultGroups
                WHERE PatientId = @PatientId
                  AND ClinicId = @ClinicId
                ORDER BY TestDate DESC, TestResultGroupId DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResultGroup(reader));
            }

            return list;
        }

        public List<TestResultGroup> GetResultGroupsByVisit(int visitId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResultGroup>();

            string query = @"
                SELECT TestResultGroupId, ClinicId, PatientId, VisitId, TestDate,
                        ResultDate, LabName, ReportNo, Remarks, CreatedDate
                FROM TestResultGroups
                WHERE VisitId = @VisitId
                  AND ClinicId = @ClinicId
                ORDER BY TestDate DESC, TestResultGroupId DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@VisitId", visitId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResultGroup(reader));
            }

            return list;
        }

        public List<TestResult> GetPreviousResults(int patientId, int clinicId, 
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResult>();

            const string query = @"
                SELECT R.TestResultId, R.TestResultGroupId, R.ClinicId, R.TestPanelId, R.TestPanelCode, R.TestPanelName,
                    R.TestParameterId, R.TestParameterCode, R.TestParameterName, R.PanelDisplayOrder, R.ParameterDisplayOrder,
                    R.NormalRange, R.NormalMin, R.NormalMax, R.TestMethod, R.DataType, R.Value, R.Unit,
                    R.ResultFlag, G.VisitId, V.VisitDate, G.TestDate
                FROM TestResults R
                INNER JOIN TestResultGroups G
                  ON R.TestResultGroupId = G.TestResultGroupId
                LEFT JOIN Visits V
                  ON G.VisitId = V.VisitId
                WHERE G.PatientId = @PatientId
                AND G.ClinicId = @ClinicId
                ORDER BY
                    G.TestDate DESC,
                    R.PanelDisplayOrder,
                    R.ParameterDisplayOrder,
                    R.TestResultId";

            using SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResult(reader));
            }

            return list;
        }

        public List<TestResult> GetPreviousResults(int patientId, int clinicId, DateTime beforeDate,
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResult>();

            const string query = @"
                SELECT R.TestResultId, R.TestResultGroupId, R.ClinicId, R.TestPanelId, R.TestPanelCode, R.TestPanelName,
                    R.TestParameterId, R.TestParameterCode, R.TestParameterName, R.PanelDisplayOrder, R.ParameterDisplayOrder,
                    R.NormalRange, R.NormalMin, R.NormalMax, R.TestMethod, R.DataType, R.Value, R.Unit,
                    R.ResultFlag, G.VisitId, V.VisitDate, G.TestDate
                FROM TestResults R
                INNER JOIN TestResultGroups G
                  ON R.TestResultGroupId = G.TestResultGroupId
                LEFT JOIN Visits V
                  ON G.VisitId = V.VisitId
                WHERE G.PatientId = @PatientId
                AND G.ClinicId = @ClinicId
                AND G.TestDate < @BeforeDate
                ORDER BY
                    G.TestDate DESC,
                    R.PanelDisplayOrder,
                    R.ParameterDisplayOrder,
                    R.TestResultId";

            using SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@BeforeDate", beforeDate.Date);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResult(reader));
            }

            return list;
        }

        public List<TestResult> GetPreviousResults(int patientId, int testParameterId, int clinicId, 
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResult>();

            const string query = @"
                SELECT R.TestResultId, R.TestResultGroupId, R.ClinicId, R.TestPanelId, R.TestPanelCode, R.TestPanelName,
                    R.TestParameterId, R.TestParameterCode, R.TestParameterName, R.PanelDisplayOrder, R.ParameterDisplayOrder,
                    R.NormalRange, R.NormalMin, R.NormalMax, R.TestMethod, R.DataType, R.Value, R.Unit,
                    R.ResultFlag, G.VisitId, V.VisitDate, G.TestDate
                FROM TestResults R
                INNER JOIN TestResultGroups G
                  ON R.TestResultGroupId = G.TestResultGroupId
                LEFT JOIN Visits V
                  ON G.VisitId = V.VisitId
                WHERE G.PatientId = @PatientId
                AND R.TestParameterId = @TestParameterId
                AND G.ClinicId = @ClinicId
                ORDER BY
                    G.TestDate DESC,
                    R.PanelDisplayOrder,
                    R.ParameterDisplayOrder,
                    R.TestResultId";

            using SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@TestParameterId", testParameterId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResult(reader));
            }

            return list;
        }

        public List<TestResult> GetPreviousResults(int patientId, int testParameterId, int clinicId, DateTime beforeDate,
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestResult>();

            const string query = @"
                SELECT R.TestResultId, R.TestResultGroupId, R.ClinicId, R.TestPanelId, R.TestPanelCode, R.TestPanelName,
                    R.TestParameterId, R.TestParameterCode, R.TestParameterName, R.PanelDisplayOrder, R.ParameterDisplayOrder,
                    R.NormalRange, R.NormalMin, R.NormalMax, R.TestMethod, R.DataType, R.Value, R.Unit,
                    R.ResultFlag, G.VisitId, V.VisitDate, G.TestDate
                FROM TestResults R
                INNER JOIN TestResultGroups G
                  ON R.TestResultGroupId = G.TestResultGroupId
                LEFT JOIN Visits V
                  ON G.VisitId = V.VisitId
                WHERE G.PatientId = @PatientId
                AND R.TestParameterId = @TestParameterId
                AND G.ClinicId = @ClinicId
                AND G.TestDate < @BeforeDate
                ORDER BY
                    G.TestDate DESC,
                    R.PanelDisplayOrder,
                    R.ParameterDisplayOrder,
                    R.TestResultId";

            using SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@TestParameterId", testParameterId);
            cmd.Parameters.AddWithValue("@BeforeDate", beforeDate.Date);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestResult(reader));
            }

            return list;
        }

        public TestResult GetPreviousResult(int patientId, int testParameterId, int clinicId, DateTime beforeDate,
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            const string query = @"
                SELECT TOP 1
                    R.TestResultId, R.TestResultGroupId, R.ClinicId, R.TestPanelId, R.TestPanelCode, R.TestPanelName,
                    R.TestParameterId, R.TestParameterCode, R.TestParameterName, R.PanelDisplayOrder, R.ParameterDisplayOrder,
                    R.NormalRange, R.NormalMin, R.NormalMax, R.TestMethod, R.DataType, R.Value, R.Unit,
                    R.ResultFlag, G.VisitId, V.VisitDate, G.TestDate
                FROM TestResults R
                INNER JOIN TestResultGroups G
                  ON R.TestResultGroupId = G.TestResultGroupId
                LEFT JOIN Visits V
                  ON G.VisitId = V.VisitId
                WHERE G.PatientId = @PatientId
                AND R.TestParameterId = @TestParameterId
                AND G.ClinicId = @ClinicId
                AND G.TestDate < @BeforeDate
                ORDER BY
                    G.TestDate DESC,
                    R.TestResultId";

            using SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@TestParameterId", testParameterId);
            cmd.Parameters.AddWithValue("@BeforeDate", beforeDate.Date);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapTestResult(reader);
            }

            return null;
        }

        public List<TestResult> GetLatestResultsByParameter(int patientId, int clinicId,
                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            List<TestResult> results = new List<TestResult>();

            const string query = @"
                    WITH LatestResults AS
                    (
                        SELECT tg.TestDate, tg.VisitId, V.VisitDate, tr.*,

                            ROW_NUMBER() OVER
                            (
                                PARTITION BY tr.TestParameterId
                                ORDER BY
                                    tg.TestDate DESC,
                                    tr.TestResultId DESC
                            ) AS RN

                        FROM TestResults tr
                        INNER JOIN TestResultGroups tg
                            ON tr.TestResultGroupId = tg.TestResultGroupId
                        LEFT JOIN Visits V
                            ON tg.VisitId = V.VisitId

                        WHERE tg.PatientId = @PatientId
                        AND tr.ClinicId = @ClinicId
                    )

                    SELECT *
                    FROM LatestResults
                    WHERE RN = 1
                    ORDER BY
                        PanelDisplayOrder,
                        ParameterDisplayOrder;";

            using SqlCommand cmd = new(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                 results.Add(MapTestResult(reader));
            }

            return results;
        }

        private TestResultGroup MapTestResultGroup(SqlDataReader reader)
        {
            return new TestResultGroup
            {
                TestResultGroupId = Convert.ToInt32(reader["TestResultGroupId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                PatientId = Convert.ToInt32(reader["PatientId"]),
                VisitId = reader["VisitId"] == DBNull.Value ? null : Convert.ToInt32(reader["VisitId"]),
                TestDate = Convert.ToDateTime(reader["TestDate"]),
                ResultDate = reader["ResultDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ResultDate"]),
                LabName = reader["LabName"] == DBNull.Value ? null : reader["LabName"].ToString(),
                ReportNo = reader["ReportNo"] == DBNull.Value ? null : reader["ReportNo"].ToString(),
                Remarks = reader["Remarks"] == DBNull.Value ? null : reader["Remarks"].ToString(),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
            };
        }

        private TestResult MapTestResult(SqlDataReader reader)
        {
            return new TestResult
            {
                TestResultId = Convert.ToInt32(reader["TestResultId"]),
                TestResultGroupId = Convert.ToInt32(reader["TestResultGroupId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                TestPanelId = Convert.ToInt32(reader["TestPanelId"]),
                TestPanelCode = reader["TestPanelCode"].ToString(),
                TestPanelName = reader["TestPanelName"].ToString(),
                TestParameterId = Convert.ToInt32(reader["TestParameterId"]),
                TestParameterCode = reader["TestParameterCode"].ToString(),
                TestParameterName = reader["TestParameterName"].ToString(),
                PanelDisplayOrder = Convert.ToInt32(reader["PanelDisplayOrder"]),
                ParameterDisplayOrder = Convert.ToInt32(reader["ParameterDisplayOrder"]),
                NormalRange = reader["NormalRange"] == DBNull.Value ? null : reader["NormalRange"].ToString(),
                NormalMin = reader["NormalMin"] == DBNull.Value ? null : Convert.ToDecimal(reader["NormalMin"]),
                NormalMax = reader["NormalMax"] == DBNull.Value ? null : Convert.ToDecimal(reader["NormalMax"]),
                TestMethod = reader["TestMethod"] == DBNull.Value ? null : reader["TestMethod"].ToString(),
                DataType = reader["DataType"].ToString() ?? string.Empty,
                Value = reader["Value"] == DBNull.Value ? null : reader["Value"].ToString(),
                Unit = reader["Unit"] == DBNull.Value ? null : reader["Unit"].ToString(),
                ResultFlag = reader["ResultFlag"] == DBNull.Value ? null : reader["ResultFlag"].ToString(),
                VisitId = Convert.ToInt32(reader["VisitId"]),
                VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                TestDate = Convert.ToDateTime(reader["TestDate"])
            };
        }
    }
}
