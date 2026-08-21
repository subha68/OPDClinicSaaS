using Models;

namespace ViewModels
{
    public class ReportDataSourceResultVM
    {
        public List<ReportResultColumn> Columns { get; set; } = new();
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
        public List<ReportSummaryItem> Summary { get; set; } = new();
        public string? ReportTitle { get; set; }
        public string? ReportCode { get; set; }
        public DateTime GeneratedOn { get; set; }
        public List<ReportResultFilterVM> Filters { get; set; } = new();
    }
}