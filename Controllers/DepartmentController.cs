using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DAL;
using BLL;
using Models;
using ViewModels;
using Filters;

namespace Controllers
{
    [AuthorizeRole(2)] // ClinicAdmin

    public class DepartmentController : Controller
    {
        private readonly SessionHelper _session;
        private DepartmentService _service;

        public DepartmentController(IConfiguration config, SessionHelper session)
        {
            _service = new DepartmentService(config);
            _session = session;
        }

        public ActionResult Index()
        {
            var list = _service.GetDepartments(_session.ClinicId);
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DepartmentVM model)
        {
            int clinicId = _session.ClinicId;

            if (ModelState.IsValid)
            {
                _service.CreateDepartment(model, clinicId);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}