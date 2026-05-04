using Microsoft.AspNetCore.Mvc;
using BLL;
using Models;
using Filters;

namespace Controllers
{
    [AuthorizeRole(1)] // SuperAdmin only
    public class ClinicController : Controller
    {
        private readonly ClinicService _service;

        public ClinicController(IConfiguration config)
        {
            _service = new ClinicService(config);
        }

        public IActionResult Index()
        {
            var clinics = _service.GetClinics();
            return View(clinics);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
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

            _service.CreateClinic(clinic);

            return RedirectToAction("Index");
        }
    }
}