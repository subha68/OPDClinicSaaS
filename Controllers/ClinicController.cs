using Microsoft.AspNetCore.Mvc;
using BLL;
using Models;
using Filters;

namespace Controllers
{
    [AuthorizeRole(1)] // SuperAdmin only
    public class ClinicController : Controller
    {
        private readonly ClinicService _clinic;
        private readonly SessionHelper _session;

        public ClinicController(IConfiguration config, SessionHelper session)
        {
            _clinic = new ClinicService(config);
            _session = session;
        }

        public IActionResult Index()
        {
            int? clinicId = _session.ClinicId;
            string roleName = _session.RoleName;
            var clinics = roleName == "SuperAdmin" ? _clinic.List() : new List<Clinic> { _clinic.Get(clinicId.Value) };
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

            _clinic.Add(clinic);

            return RedirectToAction("Index");
        }
    }
}