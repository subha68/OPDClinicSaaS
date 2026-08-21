namespace Models
{
    public class ReportFilter
    {
        public long ReportFilterId { get; set; }
        public long ReportId { get; set; }
        public string FilterCode { get; set; } = "";
        public string FilterLabel { get; set; } = "";
        public string ParameterName { get; set; } = "";
        public string DataType { get; set; } = "";
        public string ControlType { get; set; } = "";
        public string LookupCode { get; set; } = "";
        public string? DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        public string? ValidationRule { get; set; }
        public string? ValidationValue { get; set; }
    }
}