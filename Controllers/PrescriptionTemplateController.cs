using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ViewModels;
using DAL;
using BLL;
using Models;
using Helpers;

namespace Controllers
{
    public class PrescriptionTemplateController : Controller
    {
        private readonly PrescriptionTemplateService _template;

        private readonly DepartmentService _department;

        public PrescriptionTemplateController(PrescriptionTemplateService template, 
                                            DepartmentService department)
        {
            _template = template;
            _department = department;
        }

        public IActionResult Index()
        {
            return View(_template.List());
        }

        public IActionResult IndexByDepartment(int departmentId)
        {
            return View(_template.List());
        }

        public IActionResult SectionIndex(int templateId)
        {
            var sections = _template.GetSections(templateId);
            return View(sections);
        }

        public IActionResult FieldIndex(int sectionId)
        {
            var fields = _template.GetFieldsForSection(sectionId);
            return View(fields);
        }

        public IActionResult OptionIndex(int fieldId)
        {
            var options = _template.GetOptionsForField(fieldId);
            return View(options);
        }

        public IActionResult Create()
        {
            ViewBag.Departments =
                new SelectList(
                    _department.List(),
                    "DepartmentId",
                    "Name");

            return View(new PrescriptionTemplate());
        }

        [HttpPost]
        public IActionResult Create(PrescriptionTemplate model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _template.Create(model);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var model = _template.Get(id);

            ViewBag.Departments =
                new SelectList(
                    _department.List(),
                    "DepartmentId",
                    "Name",
                    model.DepartmentId);

            return View("Create", model);
        }

        [HttpPost]
        public IActionResult Edit(PrescriptionTemplate model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _template.Modify(model);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            _template.Remove(id);

            return RedirectToAction(nameof(Index));
        }
    }

}