namespace Models
{
    public class ConsultationVital
    {
        public int ConsultationVitalId { get; set; }
        public int ClinicId { get; set; }
        public int RowId { get; set; }
        public int VitalId { get; set; }
        public int VitalDefId { get; set; }
        public int VisitId { get; set; }
        public DateTime VisitDate { get; set; }
        public string VitalLabel { get; set; } = string.Empty;
        public string VitalCode { get; set; } = string.Empty;
        public string VitalType { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string? VitalUnit { get; set; }
        public string? VitalValue { get; set; }
        public bool IsDerived => VitalType == "Derived";
        public bool IsComposition => VitalType == "Composition";

        public string? DeriveFormula { get; set; }
        public string? DisplayComposition { get; set; }
        public int? DisplayOrder { get; set; }

        // Navigation Properties

        public ConsultationRow? Row { get; set; }
        public Vital? Vital { get; set; }

        public ConsultationVital()
        {
            // default constructor
        }

        public ConsultationVital(Vital vital)
        {
            ClinicId = vital.ClinicId;
            //RowId = rowId;
            VitalId = vital.VitalId;
            VitalDefId = vital.VitalDefId;
            VisitId = vital.VisitId;
            //VisitDate = visitDate;
            VitalLabel = vital.VitalLabel;
            VitalCode = vital.VitalCode;
            VitalType = vital.VitalType;
            DataType = vital.DataType;
            VitalUnit = vital.VitalUnit;
            VitalValue = vital.VitalValue;
            DeriveFormula = vital.DeriveFormula;
            DisplayComposition = vital.DisplayComposition;
            DisplayOrder = vital.DisplayOrder;
        }

        public ConsultationVital(VitalDefinition vitalDef, int rowId, int clinicId, int visitId, DateTime visitDate)
        {
            VitalDefId = vitalDef.VitalDefId;
            ClinicId = vitalDef.ClinicId ?? clinicId;
            RowId = rowId;
            VisitId = visitId;
            VisitDate = visitDate;
            VitalLabel = vitalDef.VitalLabel;
            VitalCode = vitalDef.VitalCode;
            VitalType = vitalDef.VitalType;
            DataType = vitalDef.DataType;
            VitalUnit = vitalDef.VitalUnit;
            DeriveFormula = vitalDef.DeriveFormula;
            DisplayComposition = vitalDef.DisplayComposition;
            DisplayOrder = vitalDef.DisplayOrder;
        }
    }
}