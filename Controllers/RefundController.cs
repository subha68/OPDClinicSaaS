using Microsoft.AspNetCore.Mvc;
using BLL;
using Models;
using ViewModels;

namespace Controllers
{
    public class RefundController : Controller
    {
        private readonly RefundService _refund;
        private readonly BillService _billing;

        public RefundController(RefundService refund, BillService billing)
        {
            _refund = refund;
            _billing = billing;
        }

        public IActionResult Index(int? patientId)
        {
            RefundVM vm = new();

            if (patientId.HasValue)
            {
                vm.PatientId = patientId.Value;
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult ListByPatient(int patientId)
        {
            var refunds = _refund.ListByPatient(patientId);

            List<RefundListItemVM> list = new List<RefundListItemVM>();

            foreach (var refund in refunds)
            {
                list.Add(new RefundListItemVM(refund));
            }

            return PartialView("_RefundList", list);
        }

        [HttpGet]
        public IActionResult Create(int patientId)
        {
            RefundCreateVM vm = new()
            {
                Refund = new Refund
                {
                    PatientId = patientId,
                    RefundDate = DateTime.Today,
                    RefundStatus = RefundStatus.Draft
                },

                Bills = _billing.ListRefundableBillsByPatient(patientId)
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(RefundCreateVM vm, string action)
        {
            if (vm == null)
                return Content("Model is NULL");

            var refund = vm.Refund;
            
            if(action == "submit")
            {
                if(!refund.Lines.Any())
                {
                    ModelState.AddModelError("", "At least one refund line is required.");
                }

                if(string.IsNullOrWhiteSpace(refund.Reason))
                {
                    ModelState.AddModelError(nameof(refund.Reason), "Refund reason is required.");
                }

                refund.RefundStatus = RefundStatus.PendingApproval;
            }
            else
            {
                refund.RefundStatus = RefundStatus.Draft;
            }

            if (!ModelState.IsValid)
            {
                vm.Bills = _billing.ListRefundableBillsByPatient(vm.Refund.PatientId!.Value);
                return View("Create", vm);
            }

            _refund.Create(refund);

            return RedirectToAction(nameof(Index),
                                        new
                                        {
                                            patientId = vm.Refund.PatientId
                                        });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Refund refund = _refund.GetRefund(id);

            if (refund == null)
                return NotFound();

            RefundCreateVM vm = new()
            {
                Refund = refund,
                Bills = _billing.ListRefundableBillsByPatient(refund.PatientId!.Value, refund.BillId),
            };

            return View("Create", vm);
        }

        [HttpPost]
        public IActionResult Edit(RefundCreateVM vm, string action)
        {
            if (vm == null)
                return Content("Model is NULL");

            if(action == "submit")
            {
                if(!vm.Refund.Lines.Any())
                {
                    ModelState.AddModelError("", "At least one refund line is required.");
                }

                if(string.IsNullOrWhiteSpace(vm.Refund.Reason))
                {
                    ModelState.AddModelError(nameof(vm.Refund.Reason), "Refund reason is required.");
                }

                vm.Refund.RefundStatus = RefundStatus.PendingApproval;
            }
            else if (action == "cancel")
            {
                vm.Refund.RefundStatus = RefundStatus.Cancelled;
            }
            else
            {
                vm.Refund.RefundStatus = RefundStatus.Draft;
            }

            if (!ModelState.IsValid)
            {
                vm.Bills = _billing.ListRefundableBillsByPatient(vm.Refund.PatientId!.Value);
                return View("Create", vm);
            }

            _refund.Modify(vm.Refund);

            return RedirectToAction(nameof(Index),
                                        new
                                        {
                                            patientId = vm.Refund.PatientId
                                        });
        }

        [HttpPost]
        public IActionResult SubmitForApproval(int id, int patientId)
        {
            try
            {
                _refund.SubmitForApproval(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { patientId });
        }

        [HttpGet]
        public IActionResult Approval()
        {
            var pendingRefunds = _refund.ListPendingApproval();

            if (pendingRefunds.Count == 1)
            {
                pendingRefunds[0] = _refund.GetPendingApproval(pendingRefunds[0].RefundId);
            }

            var vm = new RefundApprovalVM
            {
                PendingRefunds = pendingRefunds
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult ApprovalDetails(int id)
        {
            try
            {
                var refund = _refund.GetPendingApproval(id);

                return PartialView("_RefundApprovalDetails", refund);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            try
            {
                _refund.Approve(id);

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult Reject(int id, string reason)
        {
            try
            {
                _refund.Reject(id, reason);

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult Cancel(int id, string reason, int patientId)
        {
            try
            {
                _refund.Cancel(id, reason);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { patientId });
        }

        public IActionResult Drafts()
        {
            RefundVM vm = new RefundVM();
            vm.Refunds = _refund.ListByStatus(RefundStatus.Draft);

            return View("Index", vm);
        }

        public IActionResult PendingApproval()
        {
            RefundVM vm = new RefundVM();
            vm.Refunds = _refund.ListByStatus(RefundStatus.PendingApproval);

            return View("Index", vm);
        }

        public IActionResult Approved()
        {
            RefundVM vm = new RefundVM();
            vm.Refunds = _refund.ListApprovedNotCompleted();

            return View("Index", vm);
        }

        public IActionResult Completed()
        {
            RefundVM vm = new RefundVM();
            vm.Refunds = _refund.ListByStatus(RefundStatus.Completed);

            return View("Index", vm);
        }

        public IActionResult Cancelled()
        {
            RefundVM vm = new RefundVM();
            vm.Refunds = _refund.ListByStatus(RefundStatus.Cancelled);

            return View("Index", vm);
        }

        [HttpGet]
        public IActionResult PrepareBillHeader(int billId)
        {
            var vm = _refund.PrepareBillHeader(billId);

            if (vm == null)
                return NotFound();

            return Json(vm);
        }

        [HttpGet]
        public IActionResult RefundLineTemplate(int billId, int billLineId, int index, bool isDraft)
        {
            var line = _refund.PrepareRefundLine(billId, billLineId);

            ViewData["IsDraft"] = isDraft;
            ViewData["IsReadOnly"] = !isDraft;

            ViewData.TemplateInfo.HtmlFieldPrefix = $"Refund.Lines[{index}]";

            return PartialView("~/Views/Shared/EditorTemplates/RefundLine.cshtml", line);
        }

        [HttpGet]
        public IActionResult ListBillLines(int billId)
        {
            var lines = _refund.ListRefundableBillLines(billId);

            return PartialView("_BillLineLookup", lines);
        }

        [HttpGet]
        public JsonResult GetEligibleRefundLines(int billId)
        {
            var lines = _refund.ListRefundableBillLines(billId)
                    .Select(x => new
                    {
                        billLineId = x.LineId,
                        description = x.Description
                    });

            return Json(lines);
        }

        [HttpGet]
        public IActionResult Print(int id)
        {
            var refund = _refund.GetRefund(id);

            if (refund == null)
                return NotFound();

            return View("Print", refund);
        }

    }
}