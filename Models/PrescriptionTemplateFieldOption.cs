namespace Models
{
    public class PrescriptionTemplateFieldOption
    {
        public int OptionId { get; set; }
        public int TemplateId { get; set; }
        public int SectionId { get; set; }
        public int FieldId { get; set; }
        public int ClinicId { get; set; }

        public string OptionText { get; set; }
        public string OptionValue { get; set; }
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
            OptionText = optionText;
            DisplayOrder = displayOrder;
            IsActive = true;
            IsArchived = false;
        }
    }
}
