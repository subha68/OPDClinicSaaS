namespace Models
{
    public class PrescriptionTemplateFieldOption
    {
        public int OptionId { get; set; }
        public int TemplateId { get; set; }
        public int SectionId { get; set; }
        public int FieldId { get; set; }
        public int? ClinicId { get; set; }
        public int? SourceOptionId { get; set; }
        public bool IsCustomized { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public int DisplayOrder { get; set; }

        public PrescriptionTemplateFieldOption()
        {
            // default constructor
        }

        public PrescriptionTemplateFieldOption(string optionText, int displayOrder)
        {
            OptionText = optionText;
            DisplayOrder = displayOrder;
            IsActive = true;
            IsArchived = false;
        }

        public PrescriptionTemplateFieldOption(int templateId, int sectionId, int fieldId, int clinicId,
                                                    string optionText, int displayOrder)
        {
            TemplateId = templateId;
            SectionId = sectionId;
            FieldId = fieldId;
            ClinicId = clinicId;
            SourceOptionId = null;
            IsCustomized = false;
            OptionText = optionText;
            DisplayOrder = displayOrder;
            IsActive = true;
            IsArchived = false;
        }

        public PrescriptionTemplateFieldOption(PrescriptionTemplateFieldOption source, int targetClinicId, 
                                                    int targetSectionId, int targetFieldId)
        {
            SectionId = targetSectionId;
            FieldId = targetFieldId;
            ClinicId = targetClinicId;
            OptionText = source.OptionText;
            IsActive = source.IsActive;
            IsArchived = source.IsArchived;
            DisplayOrder = source.DisplayOrder;
            SourceOptionId = source.OptionId;
            IsCustomized = false;
        }
    }
}
