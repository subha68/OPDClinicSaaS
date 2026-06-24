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
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }
}
