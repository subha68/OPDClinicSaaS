using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class AppointmentBookingVM
    {
        public int DepartmentId { get; set; }
        public int DoctorId { get; set; }

        public List<SelectListItem> Departments = new();

        public List<SelectListItem> Doctors = new();
    }
}