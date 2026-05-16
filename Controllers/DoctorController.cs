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

        public DoctorController(IConfiguration config, SessionHelper session)
        {
            _doctor = new DoctorService(config);
            _department = new DepartmentService(config);
            _user = new UserService(config);
            _role = new RoleService(config);
            _session = session;
        }

        public ActionResult Index()
        {
            int clinicId = Convert.ToInt32(_session.ClinicId);
            var list = _doctor.GetDoctors(clinicId);

            return View(list);
        }

        public ActionResult Create()
        {
            int clinicId = Convert.ToInt32(_session.ClinicId);

            var model = new DoctorVM
            {
                Departments = _department.GetDepartments(clinicId)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.Name
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DoctorVM vm)
        {
            if (!ModelState.IsValid)
            {
                if (vm.Username != null && _user.UsernameExists(vm.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                }
                vm.Departments = _department.GetDepartments(vm.ClinicId)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.Name
                    }).ToList();
                return View(vm);
            }

            int doctorRoleId =_role.GetRoleIdByName("Doctor");

            // STEP 1: create user
            var user = new User
            {
                Username = vm.Username,
                PasswordHash = vm.Password,
                RoleId = doctorRoleId,   // Doctor Role
                FirstName = vm.FirstName,
                LastName = vm.LastName
            };

            int newUserId = _user.CreateUser(user);

            // STEP 2: create doctor
            var doctor = new DoctorVM
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                DepartmentId = vm.DepartmentId,
                RegistrationNo = vm.RegistrationNo,
                Qualification = vm.Qualification,
                ClinicId = vm.ClinicId,
                Username = vm.Username,
                UserId = newUserId
            };

            _doctor.CreateDoctor(doctor, vm.ClinicId);

            return RedirectToAction("Index");
        }
    }
}