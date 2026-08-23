using BLL;
using Microsoft.AspNetCore.Mvc;
using Models;
using ViewModels;

namespace Controllers
{
    public class ReportController : Controller
    {
        private readonly ReportService _report;
        private readonly ReportLookupRegistry _lookupRegistry;

        public ReportController(ReportService reportService, ReportLookupRegistry lookupRegistry)
        {
            _report = reportService;
            _lookupRegistry = lookupRegistry;
        }

        // ------------------------------------------------------------
        // Display report page
        // ------------------------------------------------------------
        [HttpGet]
        public IActionResult Index(string reportCode)
        {
            if (string.IsNullOrWhiteSpace(reportCode))
            {
                return BadRequest("Report code is required.");
            }

            ReportDefinitionVM? definition = _report.GetReportDefinition(reportCode);

            if (definition == null)
            {
                return NotFound($"Report '{reportCode}' was not found or is inactive.");
            }

            ReportVM model = new()
            {
                Definition = definition
            };

            foreach (ReportFilter filter in definition.Filters)
            {
                if (string.IsNullOrWhiteSpace(filter.LookupCode))
                {
                    continue;
                }

                // Lookup controls such as autocomplete are loaded
                // dynamically through JavaScript.
                if (filter.ControlType.Equals("lookup", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
    
                if (model.Lookups.ContainsKey(filter.LookupCode))
                {
                    continue;
                }

                List<ReportLookupItem> items = _lookupRegistry.Get(filter.LookupCode, _report.ClinicId);

                model.Lookups.Add(filter.LookupCode, items);
            }

            return View(model);
        }

        // ------------------------------------------------------------
        // Execute report
        // ------------------------------------------------------------
        [HttpPost]
        public IActionResult Execute(string reportCode)
        {
            try
            {
                var parameters = GetReportParameters();

                var result = _report.ExecuteReport(reportCode, parameters);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
       // ============================================================
        // CHECK EXPORT PERMISSION
        // ============================================================

        [HttpGet]
        public IActionResult CanExport(long reportId)
        {
            try
            {
                bool allowed = _report.CanExport(reportId);

                return Json(new
                {
                    allowed
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "Unable to determine report permissions."
                    });
            }
        }

        [HttpPost]
        public IActionResult ExportCsv(int reportId, string reportCode)
        {
            try
            {
                var parameters = GetReportParameters();

                foreach (var parameter in parameters)
                {
                    Console.WriteLine($"BLL Export parameter: {parameter.Key} = {parameter.Value}");
                }

                byte[] csv = _report.ExportCsv(reportId, reportCode, parameters);

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                return File(csv, "text/csv", $"{reportCode}_{timestamp}.csv");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult ExportExcel(int reportId, string reportCode)
        {
            try
            {
                var parameters = GetReportParameters();

                foreach (var parameter in parameters)
                {
                    Console.WriteLine($"BLL Excel export parameter: " +
                                        $"{parameter.Key} = {parameter.Value}");
                }

                byte[] excel = _report.ExportExcel(reportId, reportCode, parameters);

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                return File(excel,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"{reportCode}_{timestamp}.xlsx");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        private Dictionary<string, object?> GetReportParameters()
        {
            var parameters = new Dictionary<string, object?>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var key in Request.Form.Keys)
            {
                if (key.Equals("reportCode",
                        StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("reportId",
                        StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("reportDateShortcut",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                parameters[key] = Request.Form[key].ToString();
            }

            return parameters;
        }

        [HttpGet]
        public IActionResult CanPrint(long reportId)
        {
            try
            {
                bool allowed = _report.CanPrint(reportId);

                return Json(new
                {
                    allowed
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "Unable to determine report print permissions."
                    });
            }
        }

        [HttpGet]
        public IActionResult SearchLookup(string lookupCode, string? term)
        {
            try
            {
                var results = _lookupRegistry.Search(lookupCode, _report.ClinicId, term);
                return Json(results);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "Unable to search lookup."
                    });
            }
        }
    }
}