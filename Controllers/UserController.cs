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
            var model = new User();

            ViewBag.Roles = GetRoles();
            ViewBag.Clinics = GetClinics();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(User model)
        {
            model.ClinicName = model.ClinicId.HasValue ? _clinic.Get(model.ClinicId.Value)?.Name : null;
            model.RoleName = _role.Get(model.RoleId).RoleName;

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
                ViewBag.Roles = GetRoles();
                ViewBag.Clinics = GetClinics();

                return View(model);
            }

            _user.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _user.Get(id);

            if (user == null)
                return NotFound();

            ViewBag.Roles = GetRoles();
            ViewBag.Clinics = GetClinics();

            return View("Create", user);   // reuse same screen
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User model)
        {
            model.ClinicName = model.ClinicId.HasValue ? _clinic.Get(model.ClinicId.Value)?.Name : null;
            model.RoleName = _role.Get(model.RoleId).RoleName;

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

                ViewBag.Roles = GetRoles();
                ViewBag.Clinics = GetClinics();

                return View("Create", model);
            }

            _user.Update(model);

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
            return _role.List()
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