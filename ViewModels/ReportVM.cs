using Models;

namespace ViewModels
{
    public class ReportVM
    {
        public ReportDefinitionVM Definition { get; set; } = null!;
        public ReportDataSourceResultVM Result { get; set; } = new();
        public Dictionary<string, List<ReportLookupItem>> Lookups { get; set; }
                                = new(StringComparer.OrdinalIgnoreCase);
    }
}