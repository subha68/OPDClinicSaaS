using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class DoctorVM
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public int UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int DepartmentId { get; set; }
        public string RegistrationNo { get; set; }
        public string Qualification { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public List<SelectListItem>? Departments { get; set; }
    }
}
