using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ViewModels;
using DAL;
using BLL;

namespace Controllers   
{
    public class AppointmentBookingController : Controller
    {
        private readonly DepartmentService _department;
        private readonly DoctorService _doctor;
        private readonly AppointmentSlotService _slot;
        private readonly SessionHelper _session;

        public AppointmentBookingController(IConfiguration configuration, SessionHelper session)
        {
            _department = new DepartmentService(configuration);
            _doctor = new DoctorService(configuration);
            _slot = new AppointmentSlotService(configuration);
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
                title = x.StartDateTime.ToString("HH:mm"), //$"{x.StartDateTime:HH:mm} - {x.EndDateTime:HH:mm}",
                //slotDate = x.SlotDate.ToString("yyyy-MM-dd"),
                start = x.StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss"), //x.StartDateTime.ToString("HH:mm"),
                end = x.EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss"), //x.EndDateTime.ToString("HH:mm"),
                //status = x.Status,
                color = x.Color
            }));
        }
    }
}