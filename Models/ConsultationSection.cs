using System.Collections.Generic;

namespace Models
{
    public class ConsultationSection
    {
        public int SectionId { get; set; }
        public int ConsultationId { get; set; }
        public int ClinicId { get; set; }
        public int TemplateId { get; set; }
        public int TemplateSectionId { get; set; }
        public string SectionName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public bool Multiline { get; set; }
        public bool IsPharmacologyAdvice { get; set; }
        public bool IsVital { get; set; }
        public bool IsTestResult { get; set; }

        public ConsultationRow TemplateRow { get; set; }
        public List<ConsultationRow> Rows { get; set; } = new();

        public bool IsMultiline => Multiline || IsPharmacologyAdvice;

        public SectionType SectionType
        {
            get
            {
                var count = 0;

                //if (Multiline) count++;
                if (IsVital) count++;
                if (IsTestResult) count++;
                if (IsPharmacologyAdvice) count++;

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

            //    if (Multiline)
            //        return SectionType.Multiline;

                return SectionType.Standard;
            }
        }

        public ConsultationSection()
        {
            // default constructor
            Rows = new List<ConsultationRow>();
        }

        public ConsultationSection(PrescriptionTemplateSection templateSection, int clinicId, PharmacologyAdvice rx = null)
        {
            ClinicId = clinicId;
            TemplateId = templateSection.TemplateId;
            TemplateSectionId = templateSection.SectionId;
            SectionName = templateSection.SectionName;
            DisplayOrder = templateSection.DisplayOrder;
            IsVisible = true;
            Multiline = templateSection.Multiline;
            IsPharmacologyAdvice = templateSection.IsPharmacologyAdvice;
            IsVital = templateSection.IsVital;
            IsTestResult = templateSection.IsTestResult;
 
            TemplateRow = new ConsultationRow();

            if (templateSection.Fields != null)
            {
                if (templateSection.SectionType == SectionType.Vital ||
                    templateSection.SectionType == SectionType.TestResult)
                {
                    Rows.Add(new ConsultationRow());

                    // do nothing else - do not copy fields / options from template
                    // retrieval of data from Vital & TestResult are handled by ConsultationService
                }
                else if (IsMultiline)
                {
                    foreach (var templateField in templateSection.Fields)
                    {
                        var field = new ConsultationField(templateField, clinicId);
                        field.HideLabel = true;
                        TemplateRow.Fields.Add(field);
                    }
                }
                else
                {
                    var row = new ConsultationRow();

                    foreach (var field in templateSection.Fields)
                    {
                        row.Fields.Add(new ConsultationField(field, clinicId));
                    }

                    Rows.Add(row);
                }
            }
        }

    }
}