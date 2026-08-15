using System;
using System.Collections.Generic;

namespace Models
{
    public class ConsultationField
    {
        public int FieldId { get; set; }  
        public int ConsultationId { get; set; }
        public int ClinicId { get; set; }
        public int TemplateId { get; set; }
        public int SectionId { get; set; }
        public int RowId { get; set; }  
        public int TemplateFieldId { get; set; }
        public string FieldLabel { get; set; }
        public string FieldTypeCode { get; set; }
        public string FieldCode { get; set; }
        public string FieldSource { get; set; }
        public int? FieldDataId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMandatory { get; set; }
        public string? FieldValue { get; set; }
        public string Unit { get; set; }
        public bool HideLabel { get; set; }
        public ConsultationRow? Row { get; set; }
        public List<ConsultationFieldOption> Options { get; set; } = new();
        
        public string LayoutClass
        {
            get
            {
                return FieldTypeCode switch
                {
                    "CheckboxGroup" => "field-option-group",
                    "RadioGroup"    => "field-option-group",
                    "TextArea"      => "field-full-width",
                    _               => "field-standard"
                };
            }
        }

        public bool IsOptionGroup => LayoutClass == "field-option-group";
        public bool IsFullWidth   => LayoutClass == "field-full-width";
        public string ColumnClass =>
                IsFullWidth
                    ? "col-12"
                    : "col-lg-6 col-xl-6";

        public string OptionLayoutClass
        {
            get
            {
                if (Options == null || Options.Count == 0)
                    return "options-1row";

                int maxLength = Options.Max(o => o.OptionText?.Length ?? 0);

                if (maxLength > 30)
                    return "options-flow";

                return Options.Count switch
                {
                    <= 4  => "options-1row",
                    <= 8  => "options-2row",
                    <= 12 => "options-3row",
                    _     => "options-flow"
                };
            }
        }

        public ConsultationField()
        {
            // Default constructor
        }

        public ConsultationField(PrescriptionTemplateField templateField, int clinicId)
        {
            ClinicId = clinicId;
            TemplateId = templateField.TemplateId;
            SectionId = templateField.SectionId;
            TemplateFieldId = templateField.FieldId;
            FieldId = 0;
            FieldLabel = templateField.FieldLabel;
            // FieldCode = templateField.FieldCode;
            FieldTypeCode = templateField.FieldTypeCode;
            FieldCode = templateField.FieldCode;
            FieldSource = templateField.FieldSource;
            DisplayOrder = templateField.DisplayOrder;
            //IsTemplateRow = true; // Mark as a template row
            //IsMandatory = templateField.IsMandatory;
            //LineNumber = 0; // Default line number for new fields
            Options = new List<ConsultationFieldOption>();
            if (templateField.Options != null)
            {
                foreach (var templateOption in templateField.Options)
                {
                    ConsultationFieldOption fieldOption = new ConsultationFieldOption(templateOption, clinicId);
                    Options.Add(fieldOption);
                }
            }
        }

        public ConsultationField Clone()
       {
            ConsultationField field = new ConsultationField
            {
                FieldId = 0,
                ConsultationId = ConsultationId,
                ClinicId = ClinicId,
                TemplateId = TemplateId,
                SectionId = SectionId,
                RowId = 0,
                TemplateFieldId = TemplateFieldId,
                DisplayOrder = DisplayOrder,
                FieldLabel = FieldLabel,
                FieldTypeCode = FieldTypeCode,
                FieldSource = FieldSource,
                FieldDataId = FieldDataId,
                IsMandatory = IsMandatory,
                Unit = Unit,
                FieldValue = null
            };

            foreach (var option in Options)
            {
                field.Options.Add(option.Clone());
            }

            return field;
        }        
    }
}