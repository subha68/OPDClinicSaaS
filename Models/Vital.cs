namespace Models
{
    public class Vital
    {
        public int VitalId { get; set; } = 0;
        public int VitalDefId { get; set; }
        public int ClinicId  { get; set; }
        public int VisitId { get; set; }
        public string VitalLabel { get; set; }
        public string VitalCode { get; set; }
        public string VitalType { get; set; }
        public string DataType { get; set; }
        public string? VitalUnit { get; set; }
        public string VitalValue { get; set; }
        public bool IsDerived { get; set; }
        public string? DeriveFormula { get; set; }
        public string? DisplayComposition { get; set; }

        public Vital()
        {
            // efault constructor
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
            IsDerived = vitalDef.IsDerived;
            DeriveFormula = vitalDef.DeriveFormula;
            DisplayComposition = vitalDef.DisplayComposition;
        }
    }
}