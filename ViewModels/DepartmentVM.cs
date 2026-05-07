using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class DepartmentVM
    {
        public int DepartmentId { get; set; }
        public int ClinicId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}