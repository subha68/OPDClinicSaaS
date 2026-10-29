using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models
{
    public class PrescriptionTemplateField
    {
        public int TemplateId { get; set; }
        public int SectionId { get; set; }
        public int FieldId { get; set; }
        public int? ClinicId { get; set; }
        public int? SourceFieldId { get; set; }
        public bool IsCustomized { get; set; }
        public string FieldLabel { get; set; } = string.Empty;
        public string FieldTypeCode { get; set; } = string.Empty;
        public string? FieldCode { get; set; } = null;
        public string? FieldSource { get; set; } = null;
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public int DisplayOrder { get; set; }
        public string? MergeWith { get; set; } = string.Empty;
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

        public PrescriptionTemplateField(int clinicId)
        {
            ClinicId = clinicId;
            FieldLabel = "New Field";
            FieldTypeCode = "Text";
            DisplayOrder = 1;
            IsActive = true;
            IsArchived = false;
            Options = new List<PrescriptionTemplateFieldOption>();
        }

        public PrescriptionTemplateField(int templateId, int sectionId, int clinicId, string fieldName,
                                            string fieldTypeCode, int displayOrder, string? fieldCode=null, string? fieldSource=null)
        {
            TemplateId = templateId;
            SectionId = sectionId;
            ClinicId = clinicId;
            SourceFieldId = null;
            IsCustomized = false;
            FieldLabel = fieldName;
            FieldTypeCode = fieldTypeCode;
            FieldCode = fieldCode;
            DisplayOrder = displayOrder;
            FieldSource = fieldSource;
            IsActive = true;
            IsArchived = false;
            Options = new List<PrescriptionTemplateFieldOption>();
        }

        public PrescriptionTemplateField(PrescriptionTemplateField source, int targetClinicId, int targetSectionId)
        {
            SectionId = targetSectionId;
            ClinicId = targetClinicId;
            FieldLabel = source.FieldLabel;
            FieldTypeCode = source.FieldTypeCode;
            FieldCode = source.FieldCode;
            FieldSource = source.FieldSource;
            DisplayOrder = source.DisplayOrder;
            IsActive = source.IsActive;
            IsArchived = source.IsArchived;
            MergeWith = source.MergeWith;
            SourceFieldId = source.FieldId;
            IsCustomized = false;
            Options = new List<PrescriptionTemplateFieldOption>();

            if (source.Options != null)
            {
                foreach (var sourceOption in source.Options)
                {
                    Options.Add(new PrescriptionTemplateFieldOption(sourceOption, targetClinicId, targetSectionId, 0));
                }
            }
        }
    }
}
