using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using DAL;
using BLL;
using Models;
using ViewModels;
using Filters;
using Helpers;

namespace Controllers
{
    //[AuthorizeRole(4)] // Doctor

    public class ConsultationController : Controller
    {
        private AppointmentService _appointment;
        private ConsultationService _consultation;
        private DoctorService _doctor;

        public ConsultationController(AppointmentService appointment, 
                                        ConsultationService consultation, 
                                        DoctorService doctor)
        {
            _appointment = appointment;
            _doctor = doctor;
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
           // _appointment.UpdateStatus(appointmentId, "In Consultation");

            return RedirectToAction("Consult", new { appointmentId });
        }

        [HttpGet]
        public IActionResult Consult(int appointmentId)
        {
            var vm = _consultation.GetConsultation(appointmentId);

            return View(vm);
        }

        [HttpPost]
        public IActionResult Save(Consultation model)
        {
            var formCount = Request.Form.Count;
            var hasFormContent = Request.HasFormContentType;
            var modelStateValid = ModelState.IsValid;

            var errors = ModelState
                .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors.Select(e => new
                {
                    Key = x.Key,
                    Error = e.ErrorMessage,
                    Exception = e.Exception?.ToString()
                }))
                .ToList();

            int consultationId = _consultation.SaveDraft(model);

            TempData["Success"] = "Draft Consultation saved";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Consult(Consultation model)
        {
            int consultationId = _consultation.CompleteConsultation(model);

            TempData["Success"] = "Consultation completed";

            return RedirectToAction("Index");
        }

        public IActionResult Preview(int appointmentId)
        {
            var vm = _consultation.GetPrescription(appointmentId);

            return View(vm);
        }

    /*    public IActionResult Download(int appointmentId)
        {
            var vm = _consultation.GetPrescription(appointmentId);

            var pdf = new PrescriptionPdfDocument(vm);

            byte[] bytes = pdf.GeneratePdf();

            return File(
                bytes,
                "application/pdf",
                $"Prescription_{appointmentId}.pdf");
        } */
    }
}