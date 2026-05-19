using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL;
using Models;
using ViewModels;

namespace Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _user;
        private readonly ClinicService _clinic;
        private readonly RoleService _role;
        private readonly SessionHelper _session;

        public UserController(UserService userService, ClinicService clinicService, RoleService roleService, SessionHelper session)
        {
            _user = userService;
            _clinic = clinicService;
            _role = roleService;
            _session = session;

        }

        public IActionResult Index()
        {
            int? clinicId = _session.ClinicId;
            string roleName = _session.RoleName;
            var users = roleName == "SuperAdmin" ? _user.List() : _user.ListByClinic(clinicId.Value);
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new UserVM
            {
                User = new User(),
                Roles = GetRoles(),
                Clinics = GetClinics()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(UserVM vm)
        {
            vm.User.ClinicName = vm.User.ClinicId.HasValue ? _clinic.Get(vm.User.ClinicId.Value)?.Name : null;
            vm.User.RoleName = _role.Get(vm.User.RoleId).RoleName;

            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var err in errors)
                {
                    Console.WriteLine($"Field: {key} → Error: {err.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                vm.Roles = GetRoles();
                vm.Clinics = GetClinics();

                return View(vm);
            }

            _user.Add(vm.User);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _user.Get(id);

            if (user == null)
                return NotFound();

            var vm = new UserVM
            {
                User = user,
                Roles = GetRoles(),
                Clinics = GetClinics()
            };

            return View("Create", vm);   // reuse same screen
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserVM vm)
        {
            vm.User.ClinicName = vm.User.ClinicId.HasValue ? _clinic.Get(vm.User.ClinicId.Value)?.Name : null;
            vm.User.RoleName = _role.Get(vm.User.RoleId).RoleName;

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

                vm.Roles = GetRoles();
                vm.Clinics = GetClinics();
                
                return View("Create", vm);
            }

            _user.Update(vm.User);

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _user.Delete(id);

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction("Index");
        }

        // 🔽 Helpers

        private List<SelectListItem> GetRoles()
        {
            return _role.GetRoles()
                    .Select(r => new SelectListItem
                    {
                        Value = r.RoleId.ToString(),
                        Text = r.RoleName
                    }).ToList();
        }

        private List<SelectListItem> GetClinics()
        {
            int? clinicId = _session.ClinicId;
            string roleName = _session.RoleName;
            var clinics = roleName == "SuperAdmin" ? _clinic.List() : new List<Clinic> { _clinic.Get(clinicId.Value) };
            
            return clinics.Select(c => new SelectListItem
                {
                    Value = c.ClinicId.ToString(),
                    Text = c.Name
                }).ToList();
        }
    }
}