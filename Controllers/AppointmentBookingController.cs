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

        public IActionResult Index(int? departmentId, int? doctorId)
        {
            var vm = new AppointmentBookingVM();

            vm.DepartmentId = departmentId ?? 0;
            vm.DoctorId = doctorId ?? 0;

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
/*        public JsonResult GetSlots(int doctorId)
        {
            var slots = _appointment.GetSlots(doctorId);

            return Json(slots.Select(x => new
            {
                slotId = x.SlotId,
                title = x.StartDateTime.ToString("HH:mm"),
                start = x.StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = x.EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                color = x.Color
            }));
        } */

        [HttpGet]
        public JsonResult GetDailySlots(int doctorId, DateTime appointmentDate)
        {
            var slots = _appointment.GetSlots(doctorId, appointmentDate);

            return Json(slots);
        }

        [HttpGet]
        public IActionResult Book(int departmentId, int doctorId, DateTime appointmentDateTime, 
                                    int patientId = 0)
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
                VisitId = 0,
                DepartmentId = departmentId,
                DoctorId = doctorId,
                AppointmentDateTime = appointmentDateTime,
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

            return RedirectToAction(
                                "Index",
                                new
                                {
                                    departmentId = vm.DepartmentId,
                                    doctorId = vm.DoctorId
                                });
        }

        [HttpGet]
        public IActionResult SearchPatient(int departmentId, int doctorId, DateTime appointmentDateTime)
        {
            return RedirectToAction(
                "Search",
                "Patient",
                new
                {
                    returnTo = "AppointmentBooking",
                    departmentId = departmentId,
                    doctorId = doctorId,
                    appointmentDateTime = appointmentDateTime
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

        [HttpPost]
        public IActionResult Cancel(int appointmentId, string cancellationReason)
        {
            _appointment.UpdateStatus(appointmentId, "Cancelled", cancellationReason);

            return Json(new
            {
                success = true
            });
        }
    }
}
