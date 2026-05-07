using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using BLL;
using Models;
using ViewModels;
using Filters;

namespace Controllers
{
    [AuthorizeRole(2)] // ClinicAdmin
    public class DoctorController : Controller
    {
        private readonly SessionHelper _session;
        private DoctorService _service;
        private DepartmentService _deptService;

        public DoctorController(IConfiguration config, SessionHelper session)
        {
            _service = new DoctorService(config);
            _deptService = new DepartmentService(config);
            _session = session;
        }

        public ActionResult Index()
        {
            int clinicId = Convert.ToInt32(_session.ClinicId);
            var list = _service.GetDoctors(clinicId);

            return View(list);
        }

        public ActionResult Create()
        {
            int clinicId = Convert.ToInt32(_session.ClinicId);

            var model = new DoctorVM
            {
                Departments = _deptService.GetDepartments(clinicId)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.Name
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DoctorVM model)
        {
            int clinicId = Convert.ToInt32(_session.ClinicId);

            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var err in errors)
                {
                    Console.WriteLine($"Field: {key} → Error: {err.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                _service.CreateDoctor(model, clinicId);
                return RedirectToAction("Index");
            }

            model.Departments = _deptService.GetDepartments(clinicId)
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.Name
                }).ToList();

            return View(model);
        }

    }
}