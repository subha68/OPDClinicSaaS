namespace Models
{
    public class Vital
    {
        public int VitalId { get; set; } = 0;
        public int VitalDefId { get; set; }
        public int ClinicId  { get; set; }
        public int VisitId { get; set; }
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

        public Vital()
        {
            // Default constructor
        }

        public Vital(VitalDefinition vitalDef, int clinicId, int visitId)
        {
            VitalDefId = vitalDef.VitalDefId;
            ClinicId = vitalDef.ClinicId ?? clinicId;
            VisitId = visitId;
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