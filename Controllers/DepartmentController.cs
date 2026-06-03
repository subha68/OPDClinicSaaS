using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DAL;
using BLL;
using Models;
using ViewModels;
using Filters;
using Helpers;

namespace Controllers
{
    [AuthorizeRole(2)] // ClinicAdmin

    public class DepartmentController : Controller
    {
        private readonly DepartmentService _department;
        private readonly ClinicService _clinic;

        public DepartmentController(DepartmentService department, 
                                    ClinicService clinic)
        {
            _department = department;
            _clinic = clinic;
        }

        public ActionResult Index()
        {
            var list = _department.List();
            return View(list);
        }

        public ActionResult Create()
        {
            var model = new Department();
            model.ClinicId = _clinic.ClinicId;
            model.ClinicName = _clinic.Get(model.ClinicId).Name;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Department model)
        {
            if (ModelState.IsValid)
            {
                _department.Create(model);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var dept = _department.Get(id);
            if (dept == null)
                return NotFound();

            return View("Create", dept);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Department model)
        {
            //model.ClinicName = _clinic.Get(model.ClinicId).Name;

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var err in errors)
                    {
                        Console.WriteLine($"Field: {key} → Error: {err.ErrorMessage}");
                    }
                }

                return View("Create", model);
            }

            _department.Modify(model);

            TempData["Success"] = "Department updated successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _department.Remove(id);

            TempData["Success"] = "Department deleted successfully.";
            return RedirectToAction("Index");
        }

    }
}