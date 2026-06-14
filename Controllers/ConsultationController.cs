using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using DAL;
using BLL;
using Models;
using ViewModels;
using Filters;
using Documents;
using Helpers;

namespace Controllers
{
    //[AuthorizeRole(4)] // Doctor

    public class ConsultationController : Controller
    {
        private AppointmentService _appointment;
        private ConsultationService _consultation;
        private DoctorService _doctor;
        private PharmacologyService _pharmacology;

        public ConsultationController(AppointmentService appointment, 
                                        ConsultationService consultation, 
                                        DoctorService doctor, 
                                        PharmacologyService pharmacology)
        {
            _appointment = appointment;
            _doctor = doctor;
            _pharmacology = pharmacology;
            _consultation = consultation;
        }

        public IActionResult Index()
        {
            List<Appointment> appointments;

            int doctorId = _doctor.GetDoctorIdByUserId(_doctor.UserId);
            appointments = _appointment.GetTodaysAppointments(doctorId);

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
            var vm = _consultation.GetConsultation(appointmentId);

            return View(vm);
        }

        [HttpPost]
        public IActionResult Consult(Consultation model)
        {
            int consultationId = _consultation.SaveConsultation(model);

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