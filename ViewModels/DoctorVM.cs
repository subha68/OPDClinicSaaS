using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class DoctorVM
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string RegistrationNo { get; set; }
        public string Qualification { get; set; }
        public List<SelectListItem>? Departments { get; set; }
    }
}
