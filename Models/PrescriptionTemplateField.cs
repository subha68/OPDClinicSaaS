using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models
{
    public class PrescriptionTemplateField
    {
        public int TemplateId { get; set; }
        public int SectionId { get; set; }
        public int FieldId { get; set; }
        public int ClinicId { get; set; }

        public string FieldLabel { get; set; }
        public string FieldTypeCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public int DisplayOrder { get; set; }
        public bool SupportOptions => 
                        FieldTypeCode == "Dropdown" ||
                        FieldTypeCode == "CheckboxGroup" ||
                        FieldTypeCode == "RadioGroup";

        public int OptionCount { get; set; }

        public List<PrescriptionTemplateFieldOption> Options { get; set; } = new();
        //public List<SelectListItem> FieldTypes { get; set; }
    }
}
