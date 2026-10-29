using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL;
using Models;
using Filters;

namespace Controllers
{
    [AuthorizeRole(2)] // ClinicAdmin
    public class DepartmentController : Controller
    {
        private readonly DepartmentService _department;
        private readonly ClinicService _clinic;

        public DepartmentController(DepartmentService department, ClinicService clinic)
        {
            _department = department;
            _clinic = clinic;
        }

        public IActionResult Index()
        {
            return View(_department.List());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var clinicId = _clinic.ClinicId;
            var clinic = _clinic.Get(clinicId);

            var model = new Department
            {
                ClinicId = clinicId,
                ClinicName = clinic?.Name
            };

            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Department model)
        {
            if (!ModelState.IsValid)
            {
                RestoreFormData(model);
                return View(model);
            }

            try
            {
                _department.Create(model);
                TempData["Success"] = "Department created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(Department.DepartmentCode), ex.Message);
                RestoreFormData(model);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var department = _department.Get(id);

            if (department == null)
                return NotFound();

            RestoreFormData(department);
            return View("Create", department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Department model)
        {
            if (!ModelState.IsValid)
            {
                RestoreFormData(model);
                return View("Create", model);
            }

            try
            {
                _department.Modify(model);
                TempData["Success"] = "Department updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(Department.DepartmentCode), ex.Message);
                RestoreFormData(model);
                return View("Create", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _department.Remove(id);
            TempData["Success"] = "Department deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void RestoreFormData(Department model)
        {
            // Do not trust a posted ClinicId for tenant scope.
            model.ClinicId = _clinic.ClinicId;

            var clinic = _clinic.Get(model.ClinicId);
            model.ClinicName = clinic?.Name;

            PopulateDropdowns(model);
        }

        private void PopulateDropdowns(Department model)
        {
            var codes = _department.ListDepartmentCodes();

            // Build explicit text/value pairs. The SelectList(IEnumerable, string)
            // overload treats its second argument as a data-value-field name,
            // not as the selected value. Since codes is a List<string>, use
            // SelectListItem objects so each option has a real value.
            var departmentCodeItems = codes
                .Select(code => new SelectListItem
                {
                    Text = code,
                    Value = code,
                    Selected = string.Equals(
                        code,
                        model.DepartmentCode,
                        StringComparison.OrdinalIgnoreCase)
                })
                .ToList();

            ViewBag.Dropdowns = new Dictionary<string, object>
            {
                ["DepartmentCode"] = departmentCodeItems
            };
        }
    }
}
