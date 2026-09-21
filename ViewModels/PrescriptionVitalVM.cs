using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class PrescriptionVitalVM
    {
        public int VitalId { get; set; }
        public int RowId { get; set; }
        public DateTime VisitDate { get; set; }
        public string VitalLabel { get; set; } = string.Empty;
        public string VitalCode { get; set; } = string.Empty;
        public string VitalType { get; set; } = string.Empty;
        public bool IsComposition => VitalType == "Composition";
        public bool IsCompositionItem => VitalType == "CompositionItem";
        public bool IsDerived => VitalType == "Derived";
        public string VitalValue { get; set; } = string.Empty;
        public string VitalUnit { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string DisplayComposition { get; set; } = string.Empty;

        public PrescriptionVitalVM()
        {
            // default constructor
        }

        public PrescriptionVitalVM(ConsultationVital model)
        {
            RowId = model.RowId;
            VisitDate = model.VisitDate;
            VitalId = model.VitalId;
            VitalLabel = model.VitalLabel;
            VitalCode = model.VitalCode;
            VitalType = model.VitalType;
            VitalValue = model.VitalValue;
            VitalUnit = model.VitalUnit;
            DisplayOrder = model.DisplayOrder ?? 0;
            DisplayComposition = model.DisplayComposition;
        }
    }
}