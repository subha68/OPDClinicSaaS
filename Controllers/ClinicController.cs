using Microsoft.AspNetCore.Mvc;
using BLL;
using Models;
using Filters;
using Helpers;

namespace Controllers
{
    [AuthorizeRole(1)] // SuperAdmin only
    public class ClinicController : Controller
    {
        private readonly ClinicService _clinic;

        public ClinicController(ClinicService clinic)
        {
            _clinic = clinic;
        }

        public IActionResult Index()
        {
            var clinics = _clinic.RoleName == "SuperAdmin" ? _clinic.List() : new List<Clinic> { _clinic.Get(_clinic.ClinicId) };
            return View(clinics);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new Clinic();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Clinic clinic)
        {
            if (clinic == null)
            {
                return Content("Model is NULL");
            }

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

                return View(clinic);
            }

            _clinic.Create(clinic);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var clinic = _clinic.Get(id);
            if (clinic == null)
            {
                return NotFound();
            }
            return View("Create", clinic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Clinic clinic)
        {
            if (clinic == null)
            {
                return Content("Model is NULL");
            }

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

                return View("Create", clinic);
            }

            _clinic.Modify(clinic);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _clinic.Remove(id);
            return RedirectToAction("Index");
        }

        // use - https://localhost:7036/Clinic/CloneDefaultTemplate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CloneDefaultTemplate()
        {
            _clinic.CloneDefaultTemplate();
            return RedirectToAction("Login", "Account");
        }

    }
}