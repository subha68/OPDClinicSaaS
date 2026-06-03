using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL;
using Models;
using ViewModels;
using Filters;
using Helpers;

public class BillingController : Controller
{
    private readonly BillService _bill;
    private readonly VisitService _visit;
    private readonly BillingChargeService _billingCharge;
    private readonly PatientService _patient;
    private readonly ClinicService _clinic;
    private readonly PaymentService _payment;

    public BillingController(BillService bill, 
                             VisitService visit, 
                             BillingChargeService billingCharge, 
                             PatientService patient, 
                             ClinicService clinic, 
                             PaymentService payment)
    {
        _bill = bill;
        _visit = visit;
        _billingCharge = billingCharge;
        _patient = patient;
        _clinic = clinic;
        _payment = payment;
    }

    public IActionResult Index()
    {
        var bills = new List<BillListVM>();
        return View(bills);
    }

    [HttpGet]
    public IActionResult SearchPatient()
    {
        return RedirectToAction(
                    "Search",
                    "Patient",
                    new
                    {
                        returnTo = "Billing"
                    });
    }

    [HttpGet]
    public IActionResult SearchByPatient(int patientId)
    {
        var bills = _bill.ListByPatient(patientId);
        ViewBag.Patient = _patient.Get(patientId);

        return View("Index", bills);
    }

    //----------------------------------------
    // GET: Billing/Create
    //----------------------------------------
    [HttpGet]
    public IActionResult Create()
    {
        var model = new Bill
        {
            BillDate = DateTime.Today
        };

        ViewBag.Patients = GetPatients();

        ViewBag.BillingCharges = GetBillingCharges();

        return View(model);
    }

    //----------------------------------------
    // POST: Billing/Create
    //----------------------------------------
    [HttpPost]
    public IActionResult Create(Bill model)
    {
        /* if (!ModelState.IsValid)
        {
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var err in errors)
                {
                    Console.WriteLine($"Field: {key} → Error: {err.ErrorMessage}");
                }
            }

            ViewBag.Patients = GetPatients();
            ViewBag.BillingCharges = GetBillingCharges();
            return View(model);
        } */

        int billId = _bill.Create(model);

        return RedirectToAction("Print", new { id = billId });
    }

    //----------------------------------------
    // AJAX: Get Visits by Patient
    //----------------------------------------
    [HttpGet]
    public IActionResult GetVisits(int patientId, DateTime billDate)
    {
        var visits = _visit.GetVisitByPatient(patientId)
            .Select(x => new
            {
                x.VisitId,
                VisitDate = x.VisitDate
            })
            .ToList();

        var sameDateVisit = visits
            .FirstOrDefault(x => x.VisitDate.Date == billDate.Date);

        return Json(new
        {
            selectedVisitId = sameDateVisit?.VisitId,
            visits = visits.Select(x => new
            {
                x.VisitId,
                text = x.VisitDate.ToString("dd-MMM-yyyy")
            })
        });
    }

    //----------------------------------------
    // AJAX: Get Billing Charge Rate
    //----------------------------------------
    [HttpGet]
    public IActionResult GetChargeRate(int id)
    {
        var rate = _billingCharge.Get(id).Rate;

        return Json(rate);
    }

    //----------------------------------------
    // Print
    //----------------------------------------
    public IActionResult Print(int id)
    {
        var bill = _bill.Get(id);

        return View(bill);
    }

    private List<SelectListItem> GetPatients()
    {
        return _patient.List()
            .Select(x => new SelectListItem
            {
                Value = x.PatientId.ToString(),
                Text = x.FirstName + " " + x.LastName
            }).ToList();
    }

    private List<SelectListItem> GetBillingCharges()
    {
            return _billingCharge.List()
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.ChargeId.ToString(),
                    Text = x.ChargeName
                }).ToList();
    }

}