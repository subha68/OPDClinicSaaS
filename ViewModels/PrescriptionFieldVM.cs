using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class PrescriptionFieldVM
    {
        public int FieldId { get; set; }
        public int RowId { get; set; }

        public string FieldLabel { get; set; } = string.Empty;
        public string FieldCode { get; set; } = string.Empty;
        public string FieldTypeCode { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string MergeWith { get; set; } = string.Empty;

        public List<PrescriptionOptionVM> Options { get; set; }
            = new List<PrescriptionOptionVM>();
    }
}