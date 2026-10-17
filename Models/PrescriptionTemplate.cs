using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models
{
    public class PrescriptionTemplate
    {
        public int TemplateId { get; set; }
        public int? ClinicId { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string? TemplateCode { get; set; }
        public int? SourceTemplateId { get; set; }
        public bool IsActive { get; set; }
        public List<PrescriptionTemplateSection> Sections { get; set; } = new();
        public List<SelectListItem> FieldTypes { get; set; } = [];

        public PrescriptionTemplate()
        {
            // default constructor
        }

        public PrescriptionTemplate(PrescriptionTemplate source, int targetClinicId, int? targetDepartmentId, 
                                                                int? targetDoctorId, string templateCode)
        {
            ClinicId = targetClinicId;
            DepartmentId = targetDepartmentId;
            DoctorId = targetDoctorId;
            TemplateCode = templateCode;
            TemplateName = source.TemplateName;
            SourceTemplateId = source.TemplateId;
            IsActive = source.IsActive;

            if (source.Sections != null)
            {
                foreach (var sourceSection in source.Sections)
                {
                    Sections.Add(new PrescriptionTemplateSection(sourceSection, targetClinicId));
                }
            }
            
        }

    }
}
