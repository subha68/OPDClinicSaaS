using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL;
using Models;
using ViewModels;

namespace Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly ClinicService _clinicService;
        private readonly RoleService _roleService;

        public UserController(UserService userService, ClinicService clinicService, RoleService roleService)
        {
            _userService = userService;
            _clinicService = clinicService;
            _roleService = roleService;
        }

        public IActionResult Index()
        {
            var users = _userService.GetUsers();
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

            _userService.CreateUser(vm.User);
            return RedirectToAction("Index");
        }

        // 🔽 Helpers

        private List<SelectListItem> GetRoles()
        {
            return _roleService.GetRoles()
                    .Select(r => new SelectListItem
                    {
                        Value = r.RoleId.ToString(),
                        Text = r.RoleName
                    }).ToList();
        }

        private List<SelectListItem> GetClinics()
        {
            return _clinicService.GetClinics()
                .Select(c => new SelectListItem
                {
                    Value = c.ClinicId.ToString(),
                    Text = c.Name
                }).ToList();
        }
    }
}