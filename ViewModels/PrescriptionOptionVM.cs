using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class PrescriptionOptionVM
    {
        public int OptionId { get; set; }
        public int FieldId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}