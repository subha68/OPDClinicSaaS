using Microsoft.AspNetCore.Mvc;
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
    private readonly AppointmentService _appointment;

    public BillingController(BillService bill,
                                VisitService visit,
                                BillingChargeService billingCharge,
                                PatientService patient,
                                ClinicService clinic,
                                PaymentService payment,
                                AppointmentService appointment)
    {
        _bill = bill;
        _visit = visit;
        _billingCharge = billingCharge;
        _patient = patient;
        _clinic = clinic;
        _payment = payment;
        _appointment = appointment;
    }

    public IActionResult Index()
    {
        var bills = _bill.List();

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


    // -------------------------------------------------
    // GET: Billing/Create
    // -------------------------------------------------

    [HttpGet]
    public IActionResult Create(int? appointmentId)
    {
        var model = _bill.GetNewBillByAppointment(appointmentId);

        return View(model);
    }


    // -------------------------------------------------
    // POST: Billing/Create
    // -------------------------------------------------

    [HttpPost]
    public IActionResult Create(BillingCreateVM model)
    {
        int billId = _bill.Create(model.Bill);

        return RedirectToAction(
            "Print",
            new
            {
                id = billId
            });
    }


    // -------------------------------------------------
    // AJAX: Get billing context
    //
    // Called whenever PatientId becomes known/changes.
    //
    // Returns:
    //     Patient
    //     Visit for patient + bill date
    //     Appointments/doctors for that visit/date
    // -------------------------------------------------

    [HttpGet]
    public IActionResult GetBillingContext(int patientId, DateTime billDate, int? appointmentId)
    {
        var vm = _bill.GetBillingContext(patientId, billDate, appointmentId);

        if (vm.Bill.Patient == null)
        {
            return Json(new
            {
                success = false,
                message = "Patient not found."
            });
        }

        return Json(new
        {
            success = true,

            patient = new
            {
                patientId = vm.Bill.PatientId,
                patientName = vm.Bill.PatientName
            },

            hasVisit = vm.Bill.VisitId > 0,

            visitId = vm.Bill.VisitId > 0 ? vm.Bill.VisitId : (int?)null,

            previousBillCount = vm.PreviousBillCount,

            consultationChargeId = vm.ConsultationChargeId,

            appointments = vm.Appointments.Select(x => new
                {
                    appointmentId = x.AppointmentId,
                    doctorId = x.DoctorId,
                    doctorName = x.DoctorName,
                    fee = x.Fee
                })
        });
    }

    // -------------------------------------------------
    // AJAX: Billing charge rate
    // -------------------------------------------------

    [HttpGet]
    public IActionResult GetChargeRate(int id)
    {
        var rate = _billingCharge.Get(id).Rate;

        return Json(rate);
    }


    // -------------------------------------------------
    // Print
    // -------------------------------------------------

    [HttpGet]
    public IActionResult Print(int id)
    {
        var bill = _bill.Get(id);

        return View(bill);
    }


    // -------------------------------------------------
    // AJAX: Previous bills for a VISIT
    //
    // Important:
    // This is intentionally VisitId, not AppointmentId.
    //
    // A visit can have multiple doctors/appointments,
    // and we want all bills belonging to the visit.
    // -------------------------------------------------

    [HttpGet]
    public IActionResult GetPreviousBills(int visitId)
    {
        var bills = _bill.ListByVisit(visitId);

        return Json(
            bills.Select(x => new
            {
                billId = x.BillId,
                billNo = x.BillNo,
                billDate = x.BillDate.ToString("dd-MMM-yyyy"),
                amount = x.NetAmount,
                paid = x.TotalPaidAmount,
                status = x.PaymentStatus
            })
        );
    }
}
