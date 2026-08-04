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
            PopulateDepartments();

            var model = new PrescriptionTemplate();

            model.FieldTypes = PopulateFieldTypes();

            return View(model);
        }

        public IActionResult GetByDepartment(int departmentId)
        {
            var model = _template.GetByDepartment(departmentId);

            model.FieldTypes = PopulateFieldTypes();

            return PartialView("_TemplateDesigner", model);
        }

        public IActionResult GetByDoctor(int doctorId)
        {
            var model = _template.GetByDoctor(doctorId);

            model.FieldTypes = PopulateFieldTypes();

            return PartialView("_TemplateDesigner", model);
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

        [HttpPost]
        public IActionResult SaveSectionOrder([FromBody] List<SectionOrderVM> model)
        {
            if (model == null || !model.Any())
            {
                return BadRequest();
            }

            _template.SaveSectionOrder(model);

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public IActionResult SaveFieldOrder([FromBody] List<FieldOrderVM> model)
        {
            if (model == null || !model.Any())
            {
                return BadRequest();
            }

            _template.SaveFieldOrder(model);

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public IActionResult SaveOptionOrder([FromBody] List<OptionOrderVM> model)
        {
            if (model == null || !model.Any())
            {
                return BadRequest();
            }

            _template.SaveOptionOrder(model);

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public IActionResult ToggleSection(int templateId, int sectionId)
        {
            if (templateId == 0 || sectionId == 0)
            {
                return BadRequest();
            }

            var model = _template.ToggleSection(templateId, sectionId);

            return Json(new
            {
                success = true,
                isActive = model.IsActive
            });
        }

        [HttpPost]
        public IActionResult ToggleField(int templateId, int fieldId)
        {
            if (templateId == 0 || fieldId == 0)
            {
                return BadRequest();
            }

            var model = _template.ToggleField(templateId, fieldId);

            return Json(new
            {
                success = true,
                isActive = model.IsActive
            });
        }

        [HttpPost]
        public IActionResult ToggleOption(int templateId, int optionId)
        {
            if (templateId == 0 || optionId == 0)
            {
                return BadRequest();
            }

            var model = _template.ToggleOption(templateId, optionId);

            return Json(new
            {
                success = true,
                isActive = model.IsActive
            });
        }

        [HttpPost]
        public IActionResult InsertSection(int templateId, int sectionId, string position, string sectionName)
        {
            if (templateId == 0 || sectionId == 0)
            {
                return BadRequest();
            }

            _template.InsertSection(templateId, sectionId, position, sectionName);

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public IActionResult InsertField(int templateId, int sectionId, int fieldId, string position, string fieldName)
        {
            if (templateId == 0 || fieldId == 0 || sectionId == 0)
            {
                return BadRequest();
            }

            var newField = _template.InsertField(templateId, sectionId, fieldId, position, fieldName);

            return PartialView("_TemplateField", newField);
        }

        [HttpPost]
        public IActionResult InsertOption(int templateId, int fieldId, int optionId,string position, string optionName)
        {
            if (templateId == 0 || fieldId == 0 || optionId == 0)
            {
                return BadRequest();
            }

            var newOption = _template.InsertOption(templateId, fieldId, optionId, position, optionName);

            return PartialView("_TemplateOption", newOption);
        }

        [HttpPost]
        public JsonResult SaveSectionName(int templateId, int sectionId, string sectionName)
        {
            sectionName = (sectionName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(sectionName))
            {
                return Json(new
                {
                    success = false,
                    message = "Section name cannot be blank."
                });
            }

            _template.SaveSectionName(templateId, sectionId, sectionName);

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public JsonResult SaveFieldName(int templateId, int fieldId, string fieldName)
        {
            fieldName = (fieldName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return Json(new
                {
                    success = false,
                    message = "Field name cannot be blank."
                });
            }

            _template.SaveFieldName(templateId, fieldId, fieldName);

            return Json(new
            {
                success = true
            });
        }    

        [HttpPost]
        public JsonResult SaveOptionName(int templateId, int optionId, string optionName)
        {
            optionName = (optionName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(optionName))
            {
                return Json(new
                {
                    success = false,
                    message = "Option name cannot be blank."
                });
            }

            _template.SaveOptionName(templateId, optionId, optionName);

            return Json(new
            {
                success = true
            });
        }    

        [HttpPost]
        public IActionResult SaveFieldType(int templateId, int fieldId, string fieldTypeCode, bool force = false)
        {
            FieldTypeChangeVM vm = _template.SaveFieldType(templateId, fieldId, fieldTypeCode, force);

            if (vm != null)
            {
                switch (vm.ResponseType)
                {
                    case "BadRequest" :
                        return BadRequest(new
                        {
                            success = vm.Success,
                            message = vm.Message
                        });

                    case "NotFound" :
                        return NotFound(new
                        {
                            success = vm.Success,
                            message = vm.Message
                        });

                    case "ConfirmationRequired" :
                        return Json(new
                        {
                            success = vm.Success,
                            requiresConfirmation = vm.RequiresConfirmation,
                            message = vm.Message
                        });

                    case "Success" :
                        return Json(new
                        {
                            success = vm.Success,
                            value = vm.Value,
                            text = vm.Text,
                            supportsOptions = vm.SupportsOptions
                        });

                    default :
                        return null;
                }
            }

            return null;
        }

        public List<SelectListItem> PopulateFieldTypes()
        {
            return _template.ListFieldTypes()
                                .Select(x => new SelectListItem
                                {
                                    Value = x.FieldTypeCode,
                                    Text = x.FieldTypeCode //x.FieldTypeName
                                })
                                .ToList();
        }

        public void PopulateDepartments(int departmentId = 0)
        {
            ViewBag.Departments =
                new SelectList(
                    _department.List(),
                    "DepartmentId",
                    "Name",
                    departmentId);
        }

        [HttpGet]
        public JsonResult SearchTemplates(string term)
        {
            var templates =
                _template.Search(term)
                        .Take(10)
                        .Select(x => new
                        {
                            id = x.TemplateId,
                            label = x.TemplateName,
                            departmentId = x.DepartmentId,
                            departmentName = x.DepartmentName
                        });

            return Json(templates);
        }

        [HttpPost]
        public IActionResult SaveSectionOptions(
            int templateId,
            int sectionId,
            bool isMultiline,
            bool isRx)
        {
            _template.SaveSectionOptions(
                templateId,
                sectionId,
                isMultiline,
                isRx);

            return Json(new
            {
                success = true
            });
        }

    }

}