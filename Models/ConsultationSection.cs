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

        public ConsultationRow TemplateRow { get; set; }
        public List<ConsultationRow> Rows { get; set; } = new();

        public bool IsMultiline => Multiline || IsPharmacologyAdvice;

        public ConsultationSection()
        {
            // default constructor
            Rows = new List<ConsultationRow>();
        }

        public ConsultationSection(
            PrescriptionTemplateSection templateSection,
            int clinicId,
            PharmacologyAdvice rx = null)
        {
            ClinicId = clinicId;
            TemplateId = templateSection.TemplateId;
            TemplateSectionId = templateSection.SectionId;
            SectionName = templateSection.SectionName;
            DisplayOrder = templateSection.DisplayOrder;
            IsVisible = true;
            Multiline = templateSection.Multiline;
            IsPharmacologyAdvice = templateSection.IsPharmacologyAdvice;

            TemplateRow = new ConsultationRow();

            if (templateSection.Fields != null)
            {
                if (!IsMultiline)
                {
                    var row = new ConsultationRow();

                    foreach (var field in templateSection.Fields)
                    {
                        row.Fields.Add(new ConsultationField(field, clinicId));
                    }

                    Rows.Add(row);
                }
                else
                {
                    foreach (var templateField in templateSection.Fields)
                    {
                        var field = new ConsultationField(templateField, clinicId);
                        field.HideLabel = true;
                        TemplateRow.Fields.Add(field);
                    }
                }
            }
        }
    }
}