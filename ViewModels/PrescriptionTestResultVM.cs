using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class PrescriptionTestResultVM
    {
        public int TestResultId { get; set; }
        public int RowId { get; set; }
        public DateTime TestDate { get; set; }
        public string TestPanelCode { get; set; } = string.Empty;
        public string TestPanelName { get; set; } = string.Empty;
        public string TestParameterCode { get; set; } = string.Empty;
        public string TestParameterName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string ResultFlag { get; set; } = string.Empty;
        public int PanelDisplayOrder { get; set; }
        public int ParameterDisplayOrder { get; set; }

    }
}