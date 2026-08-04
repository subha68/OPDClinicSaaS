using System;
using System.Collections.Generic;

namespace Models
{
    public class ConsultationRow
    {
        public int RowId { get; set; }
        public int ConsultationId { get; set; }
        public int ClinicId { get; set; }
        public int SectionId { get; set; }
        public int RowNumber { get; set; }
        public int DisplayOrder { get; set; }
        public string RowGuid { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public RowState RowState { get; set; } = RowState.Unchanged;
        public ConsultationSection? Section { get; set; }

        public List<ConsultationField> Fields { get; set; } = new();
        public int FieldCount => Fields.Count;

        public bool IsAdded => RowState == RowState.Added;
        public bool IsModified => RowState == RowState.Modified;
        public bool IsDeleted => RowState == RowState.Deleted;
        public bool IsUnchanged => RowState == RowState.Unchanged;
        public bool IsDirty =>
            RowState == RowState.Added ||
            RowState == RowState.Modified ||
            RowState == RowState.Deleted;

        public ConsultationField? GetField(int templateFieldId)
            => Fields.FirstOrDefault(f => f.TemplateFieldId == templateFieldId);

        public ConsultationRow()
        {
            // default constructor
            Fields = new List<ConsultationField>();
        }
        public ConsultationRow Clone(RowState rowState = RowState.Unchanged)
        {
            ConsultationRow row = new ConsultationRow
            {
                ConsultationId = ConsultationId,
                ClinicId = ClinicId,
                SectionId = SectionId,

                RowId = 0,
                RowNumber = 0,
                DisplayOrder = 0,

                RowGuid = Guid.NewGuid().ToString(),

                RowState = rowState
            };

            foreach (var field in Fields)
            {
                row.Fields.Add(field.Clone(rowState));
            }

            return row;
        }
    }
}