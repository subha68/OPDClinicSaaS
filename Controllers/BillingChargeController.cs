using Microsoft.AspNetCore.Mvc;
using BLL;
using Models;
using Filters;
using Helpers;

namespace Controllers
{
    public class BillingChargeController : Controller
    {
        private readonly BillingChargeService _billingCharge;

        public BillingChargeController(BillingChargeService billingCharge)
        {
            _billingCharge = billingCharge;
        }

        public IActionResult Index()
        {
            var charges = _billingCharge.List();
            return View(charges);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new BillingCharge();
            model.IsActive = true;

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(BillingCharge billingCharge)
        {
            billingCharge.ClinicId = _billingCharge.ClinicId;
            
            if (billingCharge == null)
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

                return View(billingCharge);
            }

            _billingCharge.Create(billingCharge);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var billingCharge = _billingCharge.Get(id);
            if (billingCharge == null)
            {
                return NotFound();
            }
            return View("Create", billingCharge);
        }

        [HttpPost]
        public IActionResult Edit(BillingCharge billingCharge)
        {
            if (billingCharge == null)
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

                return View(billingCharge);
            }

            _billingCharge.Modify(billingCharge);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _billingCharge.Remove(id);
            return RedirectToAction("Index");
        }
    }
}