namespace Models
{
    public class VitalDefinition
    {
        public int VitalDefId { get; set; }
        public int? ClinicId { get; set; }
        public string? ClinicName { get; set; }
        public string VitalLabel { get; set; } = string.Empty;
        public string VitalCode { get; set; } = string.Empty;
        // Editable, Derived, Composition
        public string VitalType { get; set; } = string.Empty;
        // Text, Number, Decimal
        public string DataType { get; set; } = string.Empty;
        public string? VitalUnit { get; set; }
        public bool IsDerived => VitalType == "Derived";
        public bool IsComposition => VitalType == "Composition";
        public string? DeriveFormula { get; set; }
        public string? DisplayComposition { get; set; }
        public bool IsStandard { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public int? DisplayOrder { get; set; }
    }
}