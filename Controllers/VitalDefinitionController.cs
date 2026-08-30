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
    public class VitalDefinitionController : Controller
    {
        private readonly VitalDefinitionService _vitalDef;

        public VitalDefinitionController(VitalDefinitionService vitalDef)
        {
            _vitalDef = vitalDef;
        }


        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public IActionResult Index()
        {
            List<VitalDefinition> list = _vitalDef.List();

            // ClinicAdmin gets the Clone Default Vitals button.
            ViewBag.CheckConditions = new Dictionary<string, bool>
            {
                ["CanCloneDefaults"] = IsClinicAdmin()
            };

            return View(list);
        }


        // ============================================================
        // CREATE - GET
        // ============================================================

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsSuperAdmin() && !IsClinicAdmin())
            {
                return Forbid();
            }

            return View(new VitalDefinition
            {
                IsActive = true,
                IsStandard = IsSuperAdmin()
            });
        }


        // ============================================================
        // CREATE - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VitalDefinition model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _vitalDef.Create(model);

            TempData["SuccessMessage"] = "Vital definition created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // EDIT - GET
        // ============================================================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            VitalDefinition? model = _vitalDef.Get(id);

            if (model == null)
            {
                return NotFound();
            }

            return View("Create", model);
        }


        // ============================================================
        // EDIT - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(VitalDefinition model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            bool result = _vitalDef.Modify(model);

            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Vital definition updated successfully.";

            return View("Create", model);
            //return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // DELETE - POST
        // ============================================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            bool result = _vitalDef.Remove(id);

            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Vital definition deleted successfully.";

            return RedirectToAction(nameof(Index));
        }


        // ============================================================
        // CLONE DEFAULT VITALS
        // ============================================================

        [HttpGet]
        public IActionResult CloneDefaults()
        {
            if (!IsClinicAdmin())
            {
                return Forbid();
            }

            int count = _vitalDef.CloneDefaults();

            TempData["SuccessMessage"] =
                count == 0
                    ? "All default vital definitions already exist for this clinic."
                    : $"{count} default vital definition(s) cloned successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetActiveDefinitions()
        {
            var definitions = _vitalDef
                .List()
                .Where(x => x.IsActive)
                .Select(x => new
                {
                    x.VitalDefId,
                    x.VitalCode,
                    x.VitalLabel,
                    x.VitalType,
                    x.DataType,
                    x.VitalUnit,
                    x.IsDerived,
                    x.DeriveFormula,
                    x.DisplayComposition
                })
                .ToList();

            return Json(definitions);
        }

        // ============================================================
        // ROLE HELPERS
        // ============================================================

        private bool IsSuperAdmin()
        {
            return _vitalDef.RoleName == "SuperAdmin";
        }

        private bool IsClinicAdmin()
        {
            return _vitalDef.RoleName == "ClinicAdmin";
        }

    }
}