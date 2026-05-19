using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using DAL;
using BLL;
using Models;
using ViewModels;
using Filters;
using Documents;

namespace Controllers
{
    [AuthorizeRole(4)] // Doctor

    public class ConsultationController : Controller
    {
        private readonly SessionHelper _session;
        private AppointmentService _appointment;
        private ConsultationService _consultation;
        private DoctorService _doctor;
        private PharmacologyService _pharmacology;

        public ConsultationController(IConfiguration config, SessionHelper session)
        {
            _appointment = new AppointmentService(config);
            _session = session;
            _doctor = new DoctorService(config);
            _pharmacology = new PharmacologyService(config);
            _consultation = new ConsultationService(config);
        }

        public IActionResult Index()
        {
            int doctorId = _doctor.GetDoctorIdByUserId(_session.UserId);

            var appointments = _appointment.GetTodaysAppointments(doctorId);

            return View(appointments);
        }

        public IActionResult Start(int appointmentId)
        {
            _appointment.UpdateStatus(appointmentId, "In Consultation");

            return RedirectToAction("Consult", new { appointmentId });
        }

        [HttpGet]
        public IActionResult Consult(int appointmentId)
        {
            var vm = _consultation.GetConsultationVM(appointmentId);

            return View(vm);
        }

        [HttpPost]
        public IActionResult Consult(ConsultationVM vm)
        {
            int consultationId = _consultation.SaveConsultation(vm);

            foreach(var med in vm.pharmacologyAdvices)
            {
                _pharmacology.Add(consultationId, vm.AppointmentId, med);
            }

            _appointment.UpdateStatus(vm.AppointmentId, "Completed");

            TempData["Success"] = "Consultation completed";

            return RedirectToAction("Index");
        }

        public IActionResult Preview(int appointmentId)
        {
            var vm = _consultation.GetPrescription(appointmentId);

            return View(vm);
        }

        public IActionResult Download(int appointmentId)
        {
            var vm = _consultation.GetPrescription(appointmentId);

            var pdf = new PrescriptionPdfDocument(vm);

            byte[] bytes = pdf.GeneratePdf();

            return File(
                bytes,
                "application/pdf",
                $"Prescription_{appointmentId}.pdf");
        }
    }
}