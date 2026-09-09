using BLL;
using Microsoft.AspNetCore.Mvc;
using Models;
using ViewModels;

namespace YourProject.Controllers
{
    public class TestResultController : Controller
    {
        private readonly TestResultService _testResult;
        private readonly PatientService _patient;
        private readonly VisitService _visit;
        private readonly TestDefinitionService _testDefinition;
        private readonly ILogger<TestResultController> _logger;

        public TestResultController(TestResultService testResult, PatientService patient, VisitService visit,
                                    TestDefinitionService testDefinition, ILogger<TestResultController> logger)
        {
            _testResult = testResult;
            _patient = patient;
            _visit = visit;
            _testDefinition = testDefinition;
            _logger = logger;
        }

        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ============================================================
        // ADD REPORT
        // ============================================================

        [HttpGet]
        public IActionResult Add(int patientId)
        {
            var model = new TestResultGroupVM
                {
                    PatientId = patientId,
                    Visits = _visit.GetVisitByPatient(patientId),
                    TestDate = DateTime.Today,
                    ResultDate = DateTime.Today
                };

            model.VisitId = model.Visits?.FirstOrDefault()?.VisitId;

            return PartialView("_EditReport", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(TestResultGroupVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please correct the validation errors."
                });
            }

            try
            {
                var report = BuildReport(model);

                int id = _testResult.AddTestReport(report);

                return Json(new
                {
                    success = true,
                    reportId = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding test report");

                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ============================================================
        // EDIT REPORT
        // ============================================================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var report = _testResult.GetTestReport(id);

                if (report == null)
                    return NotFound();

            /*    model.Results = report.Results
                        .Select(x => new TestResultVM
                        {
                            TestResultId = x.TestResultId,
                            TestPanelId = x.TestPanelId,
                            TestPanelCode = x.TestPanelCode,
                            TestPanelName = x.TestPanelName,
                            TestParameterId = x.TestParameterId,
                            TestParameterCode = x.TestParameterCode,
                            TestParameterName = x.TestParameterName,
                            PanelDisplayOrder = x.PanelDisplayOrder,
                            ParameterDisplayOrder = x.ParameterDisplayOrder,
                            NormalRange = x.NormalRange,
                            NormalMin = x.NormalMin,
                            NormalMax = x.NormalMax,
                            TestMethod = x.TestMethod,
                            DataType = x.DataType,
                            Value = x.Value,
                            Unit = x.Unit,
                            ResultFlag = x.ResultFlag
                        })
                        .ToList(); */

                return PartialView("_EditReport", report);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading test report {Id}", id);

                TempData["Message"] = ex.Message;

                TempData["MessageType"] = "danger";

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TestResultGroupVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var report = BuildReport(model);

                bool result = _testResult.UpdateTestReport(report);

                return Json(new
                {
                    success = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating test report");

                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ============================================================
        // DELETE REPORT
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                bool result = _testResult.DeleteTestReport(id);

                TempData["Message"] = result
                        ? "Test report deleted successfully."
                        : "Test report not found.";

                TempData["MessageType"] = result
                        ? "success"
                        : "warning";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report");

                TempData["Message"] = ex.Message;

                TempData["MessageType"] = "danger";

                return RedirectToAction(nameof(Index));
            }
        }

        // ============================================================
        // REPORTS
        // ============================================================

        [HttpGet]
        public IActionResult ListReports(int patientId)
        {
            var reports = _testResult.ListReports(patientId)
                                        .Select(x => new TestResultGroupVM(x))
                                        .ToList();

            return PartialView("_ReportList", reports);
        }

        [HttpGet]
        public IActionResult ViewReport(int id)
        {
            var report = _testResult.GetTestReport(id);

            if (report == null)
                return NotFound();

            return PartialView("_ViewReport", report);
        }

        // ============================================================
        // PREVIOUS RESULT
        // ============================================================

        [HttpGet]
        public IActionResult GetPreviousResult(int patientId, int parameterId, DateTime? beforeDate = null)
        {
            if (patientId <= 0 || parameterId <= 0)
            {
                return Json(new
                    {
                        value = "",
                        testDate = ""
                    });
            }

            if (!beforeDate.HasValue)
                beforeDate = DateTime.Today;

            try
            {
                var result = _testResult.GetPreviousResult(patientId, parameterId, beforeDate.Value);

                if (result == null)
                    return Json(new
                        {
                            value = "",
                            testDate = ""
                        });

                return Json(new
                {
                    value = result.Value,
                    testDate = result.TestDate
                                        .ToString("dd-MMM-yyyy")
                });
            }
            catch
            {
                return Json(new
                    {
                        value = "",
                        testDate = ""
                    });
            }
        }

        [HttpGet]
        public IActionResult AddResultRow(int index)
        {
            var model = new TestResultVM();

            ViewData["Index"] = index;

            return PartialView("_ResultRow", model);
        }

        // ============================================================
        // HELPERS
        // ============================================================

        private TestResultGroup MapToModel(TestResultGroupVM vm)
        {
            TestResultGroup model = new TestResultGroup(vm);

            return model;
        }

        private TestResultGroupVM MapToViewModel(TestResultGroup model)
        {
            TestResultGroupVM vm = new TestResultGroupVM(model);

            return vm;
        }

        private TestResultGroup BuildReport(TestResultGroupVM vm)
        {
            var report =
                new TestResultGroup(vm);

            report.Results =
                vm.Results
                .Select(x => new TestResult
                {
                    TestResultId = x.TestResultId,
                    TestPanelId = x.TestPanelId,
                    TestPanelCode = x.TestPanelCode,
                    TestPanelName = x.TestPanelName,
                    TestParameterId = x.TestParameterId,
                    TestParameterCode = x.TestParameterCode,
                    TestParameterName = x.TestParameterName,
                    PanelDisplayOrder = x.PanelDisplayOrder,
                    ParameterDisplayOrder = x.ParameterDisplayOrder,
                    NormalRange = x.NormalRange,
                    NormalMin = x.NormalMin,
                    NormalMax = x.NormalMax,
                    TestMethod = x.TestMethod,
                    DataType = x.DataType,
                    Value = x.Value,
                    Unit = x.Unit,
                    ResultFlag = x.ResultFlag
                })
                .ToList();

            return report;
        }
    }
}