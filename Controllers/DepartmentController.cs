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
        private DepartmentService _department;

        public DepartmentController(IConfiguration config, SessionHelper session)
        {
            _department = new DepartmentService(config);
            _session = session;
        }

        public ActionResult Index()
        {
            _department.setClinicId(_session.ClinicId);
            var list = _department.List();
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
                _department.setClinicId(clinicId);
                _department.Add(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}