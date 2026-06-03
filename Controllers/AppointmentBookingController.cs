using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ViewModels;
using DAL;
using BLL;
using Models;
using Helpers;

namespace Controllers
{
    public class AppointmentBookingController : Controller
    {
        private readonly DepartmentService _department;
        private readonly DoctorService _doctor;
        private readonly AppointmentSlotService _slot;
        private readonly AppointmentService _appointment;
        private readonly PatientService _patient;
        private readonly VisitService _visit;

        public AppointmentBookingController(DepartmentService department, 
                                            DoctorService doctor, 
                                            AppointmentSlotService slot, 
                                            AppointmentService appointment, 
                                            PatientService patient, 
                                            VisitService visit)
        {
            _department = department;
            _doctor = doctor;
            _slot = slot;
            _appointment = appointment;
            _patient = patient;
            _visit = visit;
        }

        public IActionResult Index()
        {
            var vm = new AppointmentBookingVM();

            vm.Departments = _department.List()
                .Select(x => new SelectListItem
                {
                    Value = x.DepartmentId.ToString(),
                    Text = x.Name
                }).ToList();

            return View(vm);
        }

        [HttpGet]
        public JsonResult GetDoctors(int departmentId)
        {
            var doctors = _doctor.GetDoctorsByDepartment(departmentId);

            return Json(doctors.Select(x => new
            {
                doctorId = x.DoctorId,
                doctorName = x.FirstName + " " + x.LastName
            }));
        }

        [HttpGet]
        public JsonResult GetSlots(int doctorId)
        {
            var slots = _slot.GetAvailableSlots(doctorId);

            return Json(slots.Select(x => new
            {
                slotId = x.SlotId,
                title = x.StartDateTime.ToString("HH:mm"),
                start = x.StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = x.EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                color = x.Color
            }));
        }

        [HttpGet]
        public IActionResult Book(int doctorId, DateTime appointmentDate, int slotId, string slotTime, int patientId)
        {
            string patientName = "";
            string patientPhone = "";

            if (patientId != 0)
            {
                Patient patient = _patient.Get(patientId);
                patientName = patient.FirstName + " " + patient.LastName;
                patientPhone = patient.Phone;
            }

            var vm = new AppointmentCreateVM
            {
                SlotId = slotId,
                VisitId = 0,
                DoctorId = doctorId,
                AppointmentDate = appointmentDate,
                SlotTime = slotTime,
                PatientId = patientId,
                PatientName = patientName,
                PatientPhone = patientPhone
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Book(AppointmentCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            int appointmentId = _appointment.BookAppointment(vm);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SearchPatient(int doctorId, DateTime appointmentDate, int slotId, string slotTime)
        {
            return RedirectToAction(
                "Search",
                "Patient",
                new
                {
                    returnTo = "AppointmentBooking",
                    doctorId = doctorId,
                    appointmentDate = appointmentDate,
                    slotId = slotId,
                    slotTime = slotTime
                });
        }

        [HttpGet]
        public JsonResult GetVisit(int patientId, DateTime visitDate)
        {
            var visit = _visit.GetByPatientAndDate(patientId, visitDate);

            if (visit == null)
                return Json(null);

            return Json(new
            {
                visitId = visit.VisitId,
                visitType = visit.VisitType
            });
        }
    }
}
