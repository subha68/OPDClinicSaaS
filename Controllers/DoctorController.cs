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
        private PrescriptionTemplateService _template;

        public DoctorController(DoctorService doctor, 
                                DepartmentService department, 
                                UserService user, 
                                RoleService role, 
                                ClinicService clinic,
                                PrescriptionTemplateService template)
        {
            _doctor = doctor;
            _department = department;
            _user = user;
            _role = role;
            _clinic = clinic;
            _template = template;
        }

        public ActionResult Index()
        {
            var list = _doctor.List();

            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new Doctor
            {
                CloneTemplate = false
            };

            // Initial state; JavaScript will check the selected department.
            ViewBag.HasDoctorTemplate = false;
            ViewBag.HasDepartmentTemplate = false;

            PopulateDropdowns();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Doctor model)
        {
            model.ClinicId = _doctor.ClinicId;

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "Password is required.");
            }

            if (!ModelState.IsValid)
            {
                if (model.Username != null && _user.UsernameExists(model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                }

                ViewBag.HasDoctorTemplate = false;
                ViewBag.HasDepartmentTemplate = false;

                PopulateDropdowns();

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

            // Preserve the doctor's existing template state.
            doctor.CloneTemplate = doctor.PrescriptionTemplateId.HasValue;

            ViewBag.HasDoctorTemplate = doctor.CloneTemplate;
            ViewBag.HasDepartmentTemplate = false;

            PopulateDropdowns();

            return View("Create", doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Doctor model)
        {
            model.ClinicId = _doctor.ClinicId;

            if (!ModelState.IsValid)
            {
                var existingDoctor = _doctor.Get(model.DoctorId);

                ViewBag.HasDoctorTemplate = existingDoctor?.PrescriptionTemplateId.HasValue;

                PopulateDropdowns();

                return View("Create", model);
            }

            _doctor.Modify(model);

            TempData["Success"] = "Doctor updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult HasDepartmentTemplate(int departmentId)
        {
            if (departmentId <= 0)
            {
                return Json(new { hasTemplate = false });
            }

            var template = _template.GetByDepartment(departmentId);

            return Json(new { hasTemplate = template != null });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        private void PopulateDropdowns()
        {
            ViewBag.Dropdowns = new Dictionary<string, IEnumerable<SelectListItem>>
            {
                ["DepartmentId"] = GetDepartments()
            };
        }

    }
}