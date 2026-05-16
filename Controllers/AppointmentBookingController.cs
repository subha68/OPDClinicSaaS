using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ViewModels;
using DAL;
using BLL;
using Models;

namespace Controllers
{
    public class AppointmentBookingController : Controller
    {
        private readonly DepartmentService _department;
        private readonly DoctorService _doctor;
        private readonly AppointmentSlotService _slot;
        private readonly AppointmentService _appointment;
        private readonly PatientService _patient;
        private readonly SessionHelper _session;

        public AppointmentBookingController(
            IConfiguration configuration,
            SessionHelper session)
        {
            _department = new DepartmentService(configuration);
            _doctor = new DoctorService(configuration);
            _slot = new AppointmentSlotService(configuration);
            _appointment = new AppointmentService(configuration);
            _patient = new PatientService(configuration);
            _session = session;
        }

        public IActionResult Index()
        {
            var vm = new AppointmentBookingVM();

            int clinicId = _session.ClinicId;

            vm.Departments = _department.GetDepartments(clinicId)
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
            int clinicId = _session.ClinicId;

            var doctors = _doctor.GetDoctorsByDepartment(clinicId, departmentId);

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
        public IActionResult Book(int slotId, int doctorId, DateTime date, string time)
        {
            var vm = new AppointmentCreateVM
            {
                SlotId = slotId,
                DoctorId = doctorId,
                AppointmentDate = date,
                SlotTime = time
            };

            return View(vm);
        }

        [HttpGet]
        public JsonResult SearchByMobile(string mobile)
        {
            var patient = _patient.GetByMobile(mobile);

            if (patient == null)
                return Json(null);

            return Json(new
            {
                patientId = patient.PatientId,
                name = patient.FirstName + " " + patient.LastName,
                mobile = patient.Phone
            });
        }

        [HttpPost]
        public IActionResult Book(AppointmentCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var appointment = new AppointmentVM
            {
                DoctorId = vm.DoctorId,
                ClinicId = _session.ClinicId,
                PatientId = vm.PatientId,
                AppointmentDate = vm.AppointmentDate,
                SlotTime = TimeSpan.Parse(vm.SlotTime),
                Status = "Booked"
            };

            _appointment.Insert(appointment);

            _slot.IncrementBookedCount(vm.SlotId);

            TempData["Success"] = "Appointment booked successfully";

            return RedirectToAction("Index");
        }
    }
}
