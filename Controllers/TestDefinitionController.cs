using BLL;
using Microsoft.AspNetCore.Mvc;
using Models;
using ViewModels;

namespace YourProject.Controllers
{
    public class TestDefinitionController : Controller
    {
        private readonly TestDefinitionService _testDefinition;
        private readonly ILogger<TestDefinitionController> _logger;

        public TestDefinitionController(TestDefinitionService testDefinition, ILogger<TestDefinitionController> logger)
        {
            _testDefinition = testDefinition;
            _logger = logger;
        }

        // ============================================================
        // INDEX
        // ============================================================

        [HttpGet]
        public IActionResult Index(int? expandedPanelId = null)
        {
            List<TestPanel> panels = _testDefinition.ListPanels();

            ViewBag.ExpandedPanelId = expandedPanelId;

            return View(panels);
        }

        // ============================================================
        // PANEL
        // ============================================================

        public IActionResult AddPanel()
        {
            var model = new TestPanel
                {
                    IsActive = true,
                    DisplayOrder = _testDefinition.GetNextPanelDisplayOrder()
                };

            return PartialView("_EditPanel", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPanel(TestPanel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please correct the validation errors."
                });
            }

            int panelId = _testDefinition.AddPanel(model);

            return Json(new
            {
                success = true,
                message = "Test panel added successfully.",
                panelId = panelId
            });
        }


        public IActionResult EditPanel(int panelId)
        {
            var panel = _testDefinition.GetPanel(panelId);

            if (panel == null)
                return NotFound();

            return PartialView("_EditPanel", panel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPanel(TestPanel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.TestPanelId <= 0)
                return BadRequest();

            bool result = _testDefinition.UpdatePanel(model);

            if (!result)
                return NotFound();

            return Json(new
            {
                success = true,
                message = "Test panel updated successfully."
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePanel(int id)
        {
            if (id <= 0)
                return BadRequest();

            _testDefinition.RemovePanel(id);

            TempData["SuccessMessage"] = "Test panel deleted successfully.";

            return RedirectToAction(nameof(Index));
        }


        // ============================================================
        // PARAMETER FOR PANEL
        // ============================================================

        [HttpGet]
        public IActionResult ListParameter(int panelId)
        {
            if (panelId <= 0)
                return BadRequest();

            var panel = _testDefinition.GetPanel(panelId);

            if (panel == null)
                return NotFound();

            var parameters = _testDefinition.ListParameters(panelId);

            ViewBag.Panel = panel;

            return PartialView("_TestParameters", parameters);
        }


        [HttpGet]
        public IActionResult AddParameter(int panelId)
        {
            if (panelId <= 0)
                return BadRequest();

            var panel = _testDefinition.GetPanel(panelId);

            if (panel == null)
                return NotFound();

            var model = new TestParameterVM
            {
                Parameter = new TestParameter
                {
                    IsActive = true,
                    DataType = "Numeric",
                    DisplayOrder = 0
                },

                Association = new TestPanelParameter
                {
                    TestPanelId = panelId,
                    IsActive = true,
                    DisplayOrder = 0
                }
            };

            ViewBag.Panel = panel;

            return PartialView("_EditParameter", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddParameter(TestParameterVM model)
        {
            if (model == null)
                return BadRequest();

            if (model.Parameter == null)
                return BadRequest();

            if (model.Association == null)
                return BadRequest();

            var panelId = model.Association.TestPanelId;
            
            if (panelId <= 0)
                return BadRequest();

            var panel = _testDefinition.GetPanel(panelId);

            if (panel == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Panel = panel;

                return PartialView("_EditParameter", model);
            }

            model.Parameter.DisplayOrder = model.Association.DisplayOrder;

            _testDefinition.AddParameterToPanel(panelId, model.Parameter);

            TempData["Message"] = "Test parameter added successfully.";
            TempData["MessageType"] = "success";

            return Json(new
            {
                success = true,
                panelId = panelId
            });
        }

        [HttpGet]
        public IActionResult EditParameter(int id, int panelId)
        {
            if (id <= 0 || panelId <= 0)
                return BadRequest();

            var parameter = _testDefinition.GetParameter(id);

            if (parameter == null)
                return NotFound();

            var panel = _testDefinition.GetPanel(panelId);

            if (panel == null)
                return NotFound();

            var model = new TestParameterVM
            {
                Parameter = parameter,

                Association = new TestPanelParameter
                {
                    TestPanelId = panelId,
                    TestParameterId = parameter.TestParameterId,
                    IsActive = true
                }
            };

            ViewBag.Panel = panel;

            return PartialView("_EditParameter", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditParameter(TestParameterVM model)
        {
            if (model == null)
                return BadRequest();

            if (model.Parameter == null || model.Parameter.TestParameterId <= 0)
                return BadRequest();

            if (model.Association == null || model.Association.TestPanelId <= 0)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var panel = _testDefinition.GetPanel(model.Association.TestPanelId);

                if (panel == null)
                    return NotFound();

                ViewBag.Panel = panel;

                return PartialView("_EditParameter", model);
            }

            _testDefinition.UpdateParameter(model.Association.TestPanelId, model.Parameter);

            TempData["Message"] = "Test parameter updated successfully.";
            TempData["MessageType"] = "success";

            return Json(new
            {
                success = true,
                panelId = model.Association.TestPanelId
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteParameter(int id)
        {
            if (id <= 0)
                return BadRequest();

            _testDefinition.RemoveParameter(id);

            TempData["Message"] = "Test parameter deleted successfully.";
            TempData["MessageType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AssociateParameter(int panelId)
        {
            var model = new AssociateParameterVM
                {
                    TestPanelId = panelId,
                    DisplayOrder = _testDefinition.GetNextParameterDisplayOrder(panelId)
                };

            return PartialView("_AssociateParameter", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssociateParameter(TestPanelParameter association)
        {
            _testDefinition.AssociateParameterToPanel(association);

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DissociateParameter(int panelId, int parameterId)
        {
            if (panelId <= 0 || parameterId <= 0)
                return BadRequest();

            _testDefinition.DissociateParameterFromPanel(panelId, parameterId);

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReorderPanel(int panelId, int displayOrder)
        {
            _testDefinition.ReorderPanel(panelId, displayOrder);

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReorderParameter(int panelId, int parameterId, int newDisplayOrder)
        {
            _testDefinition.ReorderParameter(panelId, parameterId, newDisplayOrder);

            return Json(new { success = true });
        }

        // ============================================================
        // PARAMETER
        // ============================================================

        [HttpGet]
        public IActionResult GetParameterPanels(int parameterId)
        {
            if (parameterId <= 0)
                return Json(new List<object>());

            var panels = _testDefinition.GetPanelsForParameter(parameterId);

            return Json(
                panels.Select(x => new
                {
                    id = x.TestPanelId,
                    code = x.Code,
                    name = x.Name
                }));
        }

        // ============================================================
        // AUTOCOMPLETE - PANEL
        // ============================================================

        [HttpGet]
        public IActionResult GetPanels(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var panels = _testDefinition.SearchPanel(term);

            var result = panels.Select(x => new
            {
                id = x.TestPanelId,
                code = x.Code,
                name = x.Name,
                description = x.Description,
                displayOrder = x.DisplayOrder,
                isActive = x.IsActive
            });

            return Json(result);
        }


        // ============================================================
        // AUTOCOMPLETE - ASSOCIATED PARAMETER
        // ============================================================

        [HttpGet]
        public IActionResult SearchParameter(string term, int? panelId = null)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var parameters = _testDefinition.SearchParameter(term, panelId);

            var result = parameters.Select(x => new
            {
                id = x.TestParameterId,
                code = x.Code,
                name = x.Name,
                description = x.Description,
                dataType = x.DataType,
                unit = x.Unit,
                normalRange = x.NormalRange,
                normalMin = x.NormalMin,
                normalMax = x.NormalMax,
                testMethod = x.TestMethod
            });

            return Json(result);
        }

        // ============================================================
        // AUTOCOMPLETE - UNASSOCIATED PARAMETER
        // ============================================================

        [HttpGet]
        public IActionResult SearchUnassociatedParameter(string term, int panelId)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var parameters = _testDefinition.SearchUnassociatedParameter(term, panelId);

            var result = parameters.Select(x => new
            {
                id = x.TestParameterId,
                code = x.Code,
                name = x.Name,
                description = x.Description,
                dataType = x.DataType,
                unit = x.Unit,
                normalRange = x.NormalRange
            });

            return Json(result);
        }

        [HttpPost]
        public IActionResult CloneTestDefinitions(int clinicId)
        {
            int rowsAffected = _testDefinition.CloneMasterDefinitions(clinicId);

            if (rowsAffected > 0)
            {
                TempData["Message"] = "Test definitions cloned successfully.";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "All Test definitions are already cloned.";
                TempData["MessageType"] = "warning";                
            }

            return RedirectToAction("Edit",
                                    "Clinic",
                                    new { id = clinicId });
        }

        // ============================================================
        // HELPERS
        // ============================================================

        private void LoadPanelForParameterView(int panelId)
        {
            ViewBag.Panel = _testDefinition.GetPanel(panelId);
        }



    }
}
