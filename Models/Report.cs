namespace Models
{
    public class Report
    {
        public long ReportId { get; set; }
        public string ReportCode { get; set; } = "";
        public string ReportName { get; set; } = "";
        public string? Description { get; set; }
        public string DataSourceCode { get; set; } = "";
        public string ReportType { get; set; } = "";
        public bool IsActive { get; set; }
    }
}