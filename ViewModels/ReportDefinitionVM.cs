using Models;

namespace ViewModels
{
    public class ReportDefinitionVM
    {
        public Report Report { get; set; } = null!;
        public List<ReportColumn> Columns { get; set; } = new();
        public List<ReportFilter> Filters { get; set; } = new();
    }
}