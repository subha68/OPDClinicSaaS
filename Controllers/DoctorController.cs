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
        private DoctorService _doctor;
        private DepartmentService _department;
        private UserService _user;
        private RoleService _role;
        private ClinicService _clinic;

        public DoctorController(IConfiguration config, SessionHelper session)
        {
            _doctor = new DoctorService(config);
            _department = new DepartmentService(config);
            _user = new UserService(config);
            _role = new RoleService(config);
            _clinic = new ClinicService(config);
            _session = session;
        }

        public ActionResult Index()
        {
            _doctor.clinicId = Convert.ToInt32(_session.ClinicId);
            var list = _doctor.List();

            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new Doctor();
            
            _department.clinicId = Convert.ToInt32(_session.ClinicId);
            ViewBag.Departments = GetDepartments();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Doctor model)
        {
            model.ClinicId = Convert.ToInt32(_session.ClinicId);

            if (!ModelState.IsValid)
            {
                if (model.Username != null && _user.UsernameExists(model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                }
                _department.clinicId = Convert.ToInt32(_session.ClinicId);
                ViewBag.Departments = GetDepartments();

                return View(model);
            }

            int doctorRoleId =_role.GetRoleByName("Doctor").RoleId;

            // STEP 1: create user
            var user = new User
            {
                Username = model.Username,
                PasswordHash = model.Password,
                RoleId = doctorRoleId,   // Doctor Role
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            int newUserId = _user.Add(user);

            // STEP 2: create doctor
            var doctor = new Doctor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DepartmentId = model.DepartmentId,
                RegistrationNo = model.RegistrationNo,
                Qualification = model.Qualification,
                ClinicId = model.ClinicId,
                Username = model.Username,
                UserId = newUserId
            };

            _doctor.Add(doctor);

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
            model.ClinicId = Convert.ToInt32(_session.ClinicId);

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

            _doctor.Update(model);

            TempData["Success"] = "Doctor updated successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _doctor.Delete(id);

            TempData["Success"] = "Doctor deleted successfully.";
            return RedirectToAction("Index");
        }

        private List<SelectListItem> GetDepartments()
        {
            _department.clinicId = Convert.ToInt32(_session.ClinicId);

            return _department.List()
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.Name
                    }).ToList();
        }

    }
}