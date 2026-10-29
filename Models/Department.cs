using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        
        [Required(ErrorMessage = "Department Code is required.")]
        [StringLength(30, ErrorMessage = "Department Code cannot exceed 30 characters.")]
        public string DepartmentCode { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public string? ClinicName { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public int? PrescriptionTemplateId { get; set; }
        public string? PrescriptionTemplateName { get; set; }
        public int? SourceTemplateId { get; set; }
    }
}