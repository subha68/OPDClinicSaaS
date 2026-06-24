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
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }

        public List<PrescriptionTemplateFieldOption> Options { get; set; } = new();
    }
}
