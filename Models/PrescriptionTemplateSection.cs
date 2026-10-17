using System.Reflection.Metadata.Ecma335;

namespace Models
{
    public class PrescriptionTemplateSection
    {
        public int TemplateId { get; set; }
        public int? ClinicId { get; set; }
        public int SectionId { get; set; }
        public int? SourceSectionId { get; set; }
        public bool IsCustomized { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public bool Multiline { get; set; }
        public bool IsVital { get; set; }
        public bool IsTestResult { get; set; }
        public bool IsPharmacologyAdvice { get; set; }

        public int DisplayOrder { get; set; }
        public List<PrescriptionTemplateField> Fields { get; set; } = new();

        public SectionType SectionType
        {
            get
            {
                var count = 0;

                if (Multiline && IsPharmacologyAdvice) count++;
                if (Multiline && !IsPharmacologyAdvice) count++;
                if (IsVital) count++;
                if (IsTestResult) count++;

                if (count > 1)
                {
                    throw new InvalidOperationException($"Section '{SectionName}' has conflicting section flags.");
                }

                if (IsPharmacologyAdvice)
                    return SectionType.Rx;

                if (IsVital)
                    return SectionType.Vital;

                if (IsTestResult)
                    return SectionType.TestResult;

                if (Multiline)
                    return SectionType.Multiline;

                return SectionType.Standard;
            }
        }

        public PrescriptionTemplateSection()
        {
            // default constructor
        }

        public PrescriptionTemplateSection(PrescriptionTemplateSection source, int targetClinicId)
        {
            ClinicId = targetClinicId;
            SectionName = source.SectionName;
            DisplayOrder = source.DisplayOrder;
            IsActive = source.IsActive;
            Multiline = source.Multiline;
            IsPharmacologyAdvice = source.IsPharmacologyAdvice;
            IsVital = source.IsVital;
            IsTestResult = source.IsTestResult;
            SourceSectionId = source.SectionId;
            IsCustomized = false;
            IsArchived = source.IsArchived;
            Fields = new List<PrescriptionTemplateField>();

            if (source.Fields != null)
            {
                foreach (var sourceField in source.Fields)
                {
                    Fields.Add(new PrescriptionTemplateField(sourceField, targetClinicId, 0));
                }
            }            
        }

    }
}
