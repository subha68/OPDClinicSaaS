namespace Models
{
    public class PrescriptionTemplateSection
    {
        public int TemplateId { get; set; }
        public int ClinicId { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        public List<PrescriptionTemplateField> Fields { get; set; } = new();
    }
}
