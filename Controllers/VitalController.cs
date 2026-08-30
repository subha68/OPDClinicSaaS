using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL;
using Models;
using ViewModels;
using Helpers;

namespace Controllers
{
    [Authorize]
    public class VitalController : Controller
    {
        private readonly VitalService _vital;

        public VitalController(VitalService vital)
        {
            _vital = vital;
        }


        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public IActionResult GetVitals(int patientId)
        {
            var result = _vital.GetByPatientAndDate(patientId);

            return Json(result);
        }

        [HttpPost]
        public IActionResult SaveVitals(int patientId, [FromBody] List<Vital> models)
        {
            var result = _vital.Upsert(patientId, models);

            return Json(new
            {
                success = true,
                changed = result
            });
        }

        [HttpGet]
        public IActionResult CalculateDerivedVitals(
            [FromQuery] Dictionary<string, string> values)
        {
            try
            {
                var result =
                    _vital.CalculateDerivedVitals(values);

                return Json(new
                {
                    success = true,
                    vitals = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Unable to calculate derived vitals."
                });
            }
        }


    }

}