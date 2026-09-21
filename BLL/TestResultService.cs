using DAL;
using Microsoft.Data.SqlClient;
using Models;
using ViewModels;

namespace BLL
{
    public class TestResultService  : BaseService<TestResult>
    {
        private readonly TestResultDAL _testResultDAL;
        private readonly VisitDAL _visitDAL;

        public TestResultService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
                                    : base(config, httpContextAccessor)
        {
            _testResultDAL = new TestResultDAL(config);
            _visitDAL = new VisitDAL(config);

            _dal = _testResultDAL;
        }

        // ============================================================
        // GET TEST REPORT BY PATIENT + DATE
        // ============================================================

        public List<TestResultGroup> GetTestReport(int patientId, DateTime testDate)
        {
            ValidateClinicId(ClinicId);

            if (patientId <= 0)
                throw new ArgumentException("Invalid PatientId.");

            using var conn = _db.GetConnection();
            conn.Open();

            List<TestResultGroup> reports = _testResultDAL.GetGroupsByPatientAndDate(patientId, testDate.Date, ClinicId, conn);

            /*
             * Load the result rows for each report.
             */
            foreach (var report in reports)
            {
                report.Results = _testResultDAL.GetByResultGroup(report.TestResultGroupId, ClinicId, conn);

                var codes = report.Results?
                                .Select(r => r.TestParameterCode)
                                .Where(c => !string.IsNullOrWhiteSpace(c))
                                .Distinct()
                                .ToList() ?? new();

                report.ParameterSummary = string.Join(", ", codes.Take(5));

                if (codes.Count > 5)
                {
                    report.ParameterSummary += " ...";
                }
            }

            return reports;
        }

        // ============================================================
        // GET SINGLE TEST REPORT
        // ============================================================

        public TestResultGroupVM GetTestReport(int testResultGroupId)
        {
            ValidateClinicId(ClinicId);

            if (testResultGroupId <= 0)
                throw new ArgumentException("Invalid TestResultGroupId.");

            using var conn = _db.GetConnection();
            conn.Open();

            TestResultGroup report = _testResultDAL.GetTestResultGroup(testResultGroupId, ClinicId, conn);

            if (report == null)
                return null;

            if (report.VisitId.HasValue && report.VisitId.Value > 0)
            {
                report.VisitDate = _visitDAL.Get(report.VisitId.Value, ClinicId, conn).VisitDate;
            }

            report.Results = _testResultDAL.GetByResultGroup(testResultGroupId, ClinicId, conn);

            TestResultGroupVM vm = new TestResultGroupVM(report);

            TestResult previousResult;

            foreach (var result in vm.Results)
            {
                previousResult = _testResultDAL.GetPreviousResult(report.PatientId, result.TestParameterId, report.ClinicId,
                                                                                    result.VisitDate, conn);

                if (previousResult != null)
                {
                    result.PreviousValue = previousResult.Value;
                    result.PreviousResultDate = previousResult.VisitDate; 
                }

            }

            return vm;
        }

        // ============================================================
        // ADD TEST REPORT
        // ============================================================

        public int AddTestReport(TestResultGroup report)
        {
            ValidateClinicId(ClinicId);
            ValidateReport(report);

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                /*
                 * Never trust ClinicId coming from the client/model.
                 * The BLL-supplied clinicId is authoritative.
                 */
                report.ClinicId = ClinicId;

                int groupId = _testResultDAL.AddTestResultGroup(report, conn, tran);

                foreach (var result in report.Results)
                {
                    ValidateResult(result);

                    result.TestResultGroupId = groupId;
                    result.ClinicId = ClinicId;

                    _dal.Add(result, conn, tran);
                }

                tran.Commit();

                return groupId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // ============================================================
        // UPDATE TEST REPORT
        // ============================================================

        public bool UpdateTestReport(TestResultGroup report)
        {
            ValidateClinicId(ClinicId);

            if (report == null)
                throw new ArgumentNullException(nameof(report));

            if (report.TestResultGroupId <= 0)
                throw new ArgumentException("Invalid TestResultGroupId.");

            ValidateReport(report);

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                /*
                 * Verify ownership before modifying anything.
                 */
                var existing = _testResultDAL.GetTestResultGroup(report.TestResultGroupId, ClinicId, conn, tran);

                if (existing == null)
                {
                    throw new KeyNotFoundException("Test report not found for the specified clinic.");
                }

                report.ClinicId = ClinicId;

                bool updated = _testResultDAL.UpdateTestResultGroup(report, conn, tran);

                if (!updated)
                {
                    tran.Rollback();
                    return false;
                }

                /*
                 * Replace the child result rows as one atomic
                 * operation. This is considerably simpler and safer
                 * than trying to diff added/removed/updated results.
                 */
                _testResultDAL.DeleteByResultGroup(report.TestResultGroupId, ClinicId, conn, tran);

                foreach (var result in report.Results)
                {
                    ValidateResult(result);

                    result.TestResultGroupId = report.TestResultGroupId;
                    result.ClinicId = ClinicId;

                    _testResultDAL.Add(result, conn, tran);
                }

                tran.Commit();

                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // ============================================================
        // DELETE TEST REPORT
        // ============================================================

        public bool DeleteTestReport(int testResultGroupId)
        {
            ValidateClinicId(ClinicId);

            if (testResultGroupId <= 0)
                throw new ArgumentException("Invalid TestResultGroupId.");

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                /*
                 * Verify that the report belongs to this clinic.
                 */
                var existing = _testResultDAL.GetTestResultGroup(testResultGroupId, ClinicId, conn, tran);

                if (existing == null)
                {
                    tran.Rollback();
                    return false;
                }

                /*
                 * Delete children first because TestResults has
                 * a foreign key to TestResultGroups.
                 */
                _testResultDAL.DeleteByResultGroup(testResultGroupId, ClinicId, conn, tran);

                bool deleted = _testResultDAL.DeleteTestResultGroup(testResultGroupId, ClinicId, conn, tran);

                tran.Commit();

                return deleted;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public List<TestResult> GetPreviousResults(int patientId, DateTime? beforeDate = null)
        {
            if (patientId <= 0)
                throw new ArgumentException("Invalid PatientId.");

            if (ClinicId <= 0)
                throw new ArgumentException("Invalid ClinicId.");

            using SqlConnection conn = _db.GetConnection();
            conn.Open();

            try
            {
                if (beforeDate.HasValue)
                    return _testResultDAL.GetPreviousResults(patientId, ClinicId, beforeDate.Value, conn);
                else
                    return _testResultDAL.GetPreviousResults(patientId, ClinicId, conn);
            }
            catch
            {
                throw;
            }
        }

        public List<TestResult> GetPreviousResults(int patientId, int testParameterId, DateTime? beforeDate = null)
        {
            if (patientId <= 0)
                throw new ArgumentException("Invalid PatientId.");

            if (testParameterId <= 0)
                throw new ArgumentException("Invalid TestParameterId.");

            if (ClinicId <= 0)
                throw new ArgumentException("Invalid ClinicId.");

            using SqlConnection conn = _db.GetConnection();
            conn.Open();

            try
            {
                if (beforeDate.HasValue)
                    return _testResultDAL.GetPreviousResults(patientId, testParameterId, ClinicId, beforeDate.Value, conn);
                else
                    return _testResultDAL.GetPreviousResults(patientId, testParameterId, ClinicId, conn);
            }
            catch
            {
                throw;
            }
        }

        public TestResult? GetPreviousResult(int patientId, int testParameterId, DateTime beforeDate)
        {
            using SqlConnection conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _testResultDAL.GetPreviousResult(patientId, testParameterId, ClinicId, beforeDate, conn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public List<TestResultGroup> ListReports(int patientId)
        {
            if (patientId <= 0)
            {
                return new List<TestResultGroup>();
            }

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                List<TestResultGroup> reports = ListReports(patientId, conn);

                foreach (var report in reports)
                {
                    report.Results = _testResultDAL.GetByResultGroup(report.TestResultGroupId, ClinicId, conn);
                 
                    var codes = report.Results?
                                    .Select(r => r.TestParameterCode)
                                    .Where(c => !string.IsNullOrWhiteSpace(c))
                                    .Distinct()
                                    .ToList() ?? new();

                    report.ParameterSummary = string.Join(", ", codes.Take(5));

                    if (codes.Count > 5)
                    {
                        report.ParameterSummary += " ...";
                    }

                }

                return reports;

            }
            catch
            {
                throw;
            }
        }

        public List<TestResultGroup> ListReports(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            return _testResultDAL.GetResultGroupsByPatient(patientId, ClinicId, conn, tran);
        }

        public List<TestResultGroup> GetLatestByParameter(int patientId, int clinicId, 
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            List<TestResultGroup> groups = new List<TestResultGroup>();
            List<TestResult> results = _testResultDAL.GetLatestResultsByParameter(patientId, clinicId, conn, tran);

            if (results != null && results.Count > 0)
            {
                groups = results
                    .GroupBy(r => new
                    {
                        r.TestDate,
                        r.VisitId,
                        r.VisitDate
                    })
                    .Select(g => new TestResultGroup
                    {
                        PatientId = patientId,
                        TestDate = g.Key.TestDate,
                        VisitId = g.Key.VisitId,
                        VisitDate = g.Key.VisitDate
                    })
                    .ToList();


                foreach (var group in groups)
                {
                    group.Results = results
                        .Where(r =>
                            r.TestDate == group.TestDate &&
                            r.VisitId == group.VisitId)
                        .ToList();
                }
            }

            return groups;
        }
        

        // ============================================================
        // VALIDATION
        // ============================================================

        private void ValidateClinicId(int clinicId)
        {
            if (clinicId <= 0)
            {
                throw new ArgumentException("A valid ClinicId is required.");
            }
        }

        private void ValidateReport(TestResultGroup report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            if (report.PatientId <= 0)
                throw new ArgumentException("Invalid PatientId.");

            if (report.TestDate == default)
                throw new ArgumentException("TestDate is required.");

            if (report.Results == null || report.Results.Count == 0)
                throw new ArgumentException("At least one test result is required.");
        }

        private void ValidateResult(TestResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (result.TestPanelId <= 0)
                throw new ArgumentException("Invalid TestPanelId.");

            if (result.TestParameterId <= 0)
                throw new ArgumentException("Invalid TestParameterId.");

            if (string.IsNullOrWhiteSpace(result.TestPanelCode))
                throw new ArgumentException("TestPanelCode is required.");

            if (string.IsNullOrWhiteSpace(result.TestParameterCode))
                throw new ArgumentException("TestParameterCode is required.");

            if (string.IsNullOrWhiteSpace(result.DataType))
                throw new ArgumentException("DataType is required.");
    
        }
    }
}
