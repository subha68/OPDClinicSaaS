using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL;
using Models;
using ViewModels;
using Filters;

public class BillingController : Controller
{
    private readonly BillService _bill;
    private readonly VisitService _visit;
    private readonly BillingChargeService _billingCharge;
    private readonly PatientService _patient;
    private readonly ClinicService _clinic;
    private readonly PaymentService _payment;
    private readonly SessionHelper _session;

    public BillingController(IConfiguration config, SessionHelper session)
    {
        _bill = new BillService(config);
        _visit = new VisitService(config);
        _billingCharge = new BillingChargeService(config);
        _patient = new PatientService(config);
        _clinic = new ClinicService(config);
        _payment = new PaymentService(config);
        _session = session;
    }

    public IActionResult Index()
    {
        var bills = new List<BillListVM>();
        return View(bills);
    }

    [HttpGet]
    public IActionResult Search(string searchText)
    {
        int clinicId = _session.ClinicId;
        var patients = _patient.SearchByText(searchText, clinicId);
        var patient = _patient.GetFirstOrDefault(patients);
        var bills = _bill.ListByPatient(clinicId, patient.PatientId);

        return View("Index", bills);
    }

    [HttpGet]
    public IActionResult SearchByPatientId(int patientId)
    {
        int clinicId = _session.ClinicId;
        var bills = _bill.ListByPatient(clinicId, patientId);

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
        model.Visit = _visit.Get(model.VisitId);
        model.PatientId = model.Visit.PatientId;
        model.Patient = _patient.Get(model.PatientId);
        model.ClinicId = model.Visit.ClinicId;
        model.Clinic = _clinic.Get(model.ClinicId);

        model.GrossAmount = model.BillLines.Sum(x => x.Rate * x.Quantity);
        model.DiscountAmount = model.BillLines.Sum(x => (x.Rate * x.Quantity * x.DiscountPercent / 100));
        model.NetAmount = model.GrossAmount - model.DiscountAmount;
        model.AmountInWords = NumberToWords((int)model.NetAmount);

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

            ViewBag.Patients = GetPatients();
            ViewBag.BillingCharges = GetBillingCharges();
            return View(model);
        }

        int billId = _bill.Add(model);

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
        bill.Visit = _visit.Get(bill.VisitId);
        bill.PatientId = bill.Visit.PatientId;
        bill.Patient = _patient.Get(bill.PatientId);
        bill.ClinicId = bill.Visit.ClinicId;
        bill.Clinic = _clinic.Get(bill.ClinicId);

        bill.Payments = _payment.GetPaymentsByBill(id);
        bill.TotalPaidAmount = _payment.GetTotalPaid(id);
        bill.BalanceAmount = bill.NetAmount - bill.TotalPaidAmount;

        return View(bill);
    }

    private List<SelectListItem> GetPatients()
    {
        return _patient.List(/*_session.ClinicId*/)
            .Select(x => new SelectListItem
            {
                Value = x.PatientId.ToString(),
                Text = x.FirstName + " " + x.LastName
            }).ToList();
    }

    private List<SelectListItem> GetBillingCharges()
    {
            return _billingCharge.ListByClinic(_session.ClinicId)
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.ChargeId.ToString(),
                    Text = x.ChargeName
                }).ToList();
    }

    //----------------------------------------
    // simple number-to-words
    //----------------------------------------
    private string NumberToWords(int n)
    {
        return n.ToString(); // replace later with Humanizer if needed
    }
}