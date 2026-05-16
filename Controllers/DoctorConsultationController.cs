using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DAL;
using BLL;
using Models;
using ViewModels;
using Filters;

namespace Controllers
{
    [AuthorizeRole(4)] // Doctor

    public class DoctorConsultationController : Controller
    {
        private readonly SessionHelper _session;
        private AppointmentService _appointment;
        private DoctorService _doctor;
        private PharmacologyService _pharmacology;

        public DoctorConsultationController(IConfiguration config, SessionHelper session)
        {
            _appointment = new AppointmentService(config);
            _session = session;
            _doctor = new DoctorService(config);
            _pharmacology = new PharmacologyService(config);
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
            var vm = _appointment.GetConsultationVM(appointmentId);

            return View(vm);
        }

        [HttpPost]
        public IActionResult Consult(ConsultationVM vm)
        {
            _appointment.SaveConsultation(vm);

            foreach(var med in vm.pharmacologyAdvices)
            {
                _pharmacology.Add(vm.AppointmentId, med);
            }

            _appointment.UpdateStatus(vm.AppointmentId, "Completed");

            TempData["Success"] = "Consultation completed";

            return RedirectToAction("Index");
        }
    }
}