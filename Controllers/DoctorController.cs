using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using BLL;
using Models;
using ViewModels;
using Filters;
using Helpers;

namespace Controllers
{
    [AuthorizeRole(2)] // ClinicAdmin
    public class DoctorController : Controller
    {
        private DoctorService _doctor;
        private DepartmentService _department;
        private UserService _user;
        private RoleService _role;
        private ClinicService _clinic;

        public DoctorController(DoctorService doctor, 
                                DepartmentService department, 
                                UserService user, 
                                RoleService role, 
                                ClinicService clinic)
        {
            _doctor = doctor;
            _department = department;
            _user = user;
            _role = role;
            _clinic = clinic;
        }

        public ActionResult Index()
        {
            var list = _doctor.List();

            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new Doctor();
            
            ViewBag.Departments = GetDepartments();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Doctor model)
        {
            model.ClinicId = _doctor.ClinicId;

            if (!ModelState.IsValid)
            {
                if (model.Username != null && _user.UsernameExists(model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                }

                ViewBag.Departments = GetDepartments();

                return View(model);
            }

            _doctor.Create(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var doctor = _doctor.Get(id);

            if (doctor == null)
                return NotFound();

            ViewBag.Departments = GetDepartments();

            return View("Create", doctor);   // reuse same screen
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Doctor model)
        {
            model.ClinicId = _doctor.ClinicId;

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

                ViewBag.Departments = GetDepartments();

                return View("Create", model);
            }

            _doctor.Modify(model);

            TempData["Success"] = "Doctor updated successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _doctor.Remove(id);

            TempData["Success"] = "Doctor deleted successfully.";
            return RedirectToAction("Index");
        }

        public List<Doctor> GetDoctorsByDepartment(int DepartmentId)
        {
            return _doctor.GetDoctorsByDepartment(DepartmentId);
        }

        private List<SelectListItem> GetDepartments()
        {
            return _department.List()
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.Name
                    }).ToList();
        }

    }
}