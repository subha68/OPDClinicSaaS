using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public int ClinicId { get; set; }
        public string? ClinicName { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}