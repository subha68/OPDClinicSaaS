using Microsoft.AspNetCore.Mvc;
using BLL;
using Models;
using Filters;
using Helpers;

namespace Controllers
{
    [AuthorizeRole(1)] // SuperAdmin only
    public class RoleController : Controller
    {
        private readonly RoleService _role;

        public RoleController(RoleService role)
        {
            _role = role;
        }

        public IActionResult Index()
        {
            var roles = _role.List();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new Role();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(Role role)
        {
            if (role == null)
            {
                return Content("Model is NULL");
            }

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

                return View(role);
            }

            _role.Create(role);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var role = _role.Get(id);
            if (role == null)
            {
                return NotFound();
            }
            return View("Create", role);
        }

        [HttpPost]
        public IActionResult Edit(Role role)
        {
            if (role == null)
            {
                return Content("Model is NULL");
            }

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

                return View(role);
            }

            _role.Modify(role);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _role.Remove(id);
            return RedirectToAction("Index");
        }
    }
}