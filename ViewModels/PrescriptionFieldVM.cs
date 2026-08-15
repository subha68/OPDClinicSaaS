using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class PrescriptionFieldVM
    {
        public int FieldId { get; set; }
        public int RowId { get; set; }

        public string FieldLabel { get; set; }
        public string FieldValue { get; set; }
        public int DisplayOrder { get; set; }

        public List<PrescriptionOptionVM> Options { get; set; }
            = new List<PrescriptionOptionVM>();
    }
}