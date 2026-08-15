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
        public string FieldCode { get; set; }
        public string FieldSource { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public int DisplayOrder { get; set; }
        public bool SupportOptions => 
                        FieldTypeCode == "Dropdown" ||
                        FieldTypeCode == "CheckboxGroup" ||
                        FieldTypeCode == "RadioGroup";

        public int OptionCount { get; set; }

        public List<PrescriptionTemplateFieldOption> Options { get; set; } = new();

        public static readonly List<SelectListItem> LookupSources = new()
                        {
                            new("Medicines", "Medicines")
                        };
        
        public PrescriptionTemplateField()
        {
            // default constructor
        }

        public PrescriptionTemplateField(int templateId, int sectionId, int clinicId, string fieldName, 
                                            string fieldTypeCode, int displayOrder, string fieldCode=null, string fieldSource=null)
        {
            TemplateId = templateId;
            SectionId = sectionId;
            ClinicId = clinicId;
            FieldLabel = fieldName;
            FieldTypeCode = fieldTypeCode;
            FieldCode = fieldCode;
            DisplayOrder = displayOrder;
            FieldSource = fieldSource;
            IsActive = true;
            IsArchived = false;
            Options = new List<PrescriptionTemplateFieldOption>();
        }
    }
}
