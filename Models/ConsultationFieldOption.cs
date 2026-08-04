using System;
using System.Collections.Generic;

namespace Models
{
    public class ConsultationFieldOption
    {
        public int ConsultationId { get; set; }
        public int ClinicId { get; set; }
        public int TemplateId { get; set; }
        public int SectionId { get; set; }
        public int RowId { get; set; }
        public int FieldId { get; set; }
        public int TemplateOptionId { get; set; }
        public int OptionId { get; set; }  
        public string? OptionText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsSelected { get; set; }
        public RowState RowState { get; set; } = RowState.Unchanged;
        public bool IsAdded => RowState == RowState.Added;
        public bool IsModified => RowState == RowState.Modified;
        public bool IsDeleted => RowState == RowState.Deleted;
        public bool IsUnchanged => RowState == RowState.Unchanged;
        public bool IsDirty =>
            RowState == RowState.Added ||
            RowState == RowState.Modified ||
            RowState == RowState.Deleted;

        public ConsultationField? Field { get; set; }

        public ConsultationFieldOption()
        {
            // Default constructor
        }

        public ConsultationFieldOption(PrescriptionTemplateFieldOption templateOption, int clinicId)
        {
            ClinicId = clinicId;
            TemplateId = templateOption.TemplateId;
            ConsultationId = 0;
            SectionId = 0;
            FieldId = 0;
            TemplateOptionId = templateOption.OptionId;
            OptionId = 0;
            OptionText = templateOption.OptionText;
            DisplayOrder = templateOption.DisplayOrder;
            IsSelected = false;
        }

        public ConsultationFieldOption Clone(RowState rowState = RowState.Unchanged)
        {
            return new ConsultationFieldOption
            {
                OptionId = 0,
                ConsultationId = ConsultationId,
                ClinicId = ClinicId,
                TemplateId = TemplateId,
                SectionId = SectionId,
                FieldId = 0,
                TemplateOptionId = TemplateOptionId,
                DisplayOrder = DisplayOrder,
                OptionText = OptionText,
                IsSelected = false,
                RowState = rowState
            };
        }
    }
}