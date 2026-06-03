using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL;
using Models;
using ViewModels;
using Helpers;

namespace Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _user;
        private readonly ClinicService _clinic;
        private readonly RoleService _role;

        public UserController(UserService user, ClinicService clinic, RoleService role)
        {
            _user = user;
            _clinic = clinic;
            _role = role;
        }

        public IActionResult Index()
        {
            var users = _user.RoleName == "SuperAdmin" ? _user.List() : _user.ListByClinic();

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

            _user.Create(model);
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

            _user.Modify(model);

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _user.Remove(id);

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
            var clinics = _user.RoleName == "SuperAdmin" ? _clinic.List() : new List<Clinic> { _clinic.Get(_user.ClinicId) };
            
            return clinics.Select(c => new SelectListItem
                {
                    Value = c.ClinicId.ToString(),
                    Text = c.Name
                }).ToList();
        }
    }
}