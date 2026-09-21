using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models
{
    public class PrescriptionTemplate
    {
        public int TemplateId { get; set; }
        public int ClinicId { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? DoctorId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string? TemplateCode { get; set; }
        public int? SourceTemplateId { get; set; }
        public bool IsActive { get; set; }
        public List<PrescriptionTemplateSection> Sections { get; set; } = new();
        public List<SelectListItem> FieldTypes { get; set; } = [];

    }
}
