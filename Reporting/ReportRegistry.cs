using DAL;
using Models;

namespace BLL
{
    public static class ReportRegistry
    {
        private static readonly Dictionary<string, ReportExecutionDefinition>
        _reports = new(StringComparer.OrdinalIgnoreCase)
                        {
                            ["DAILY_COLLECTION_SUMMARY"] = new ReportExecutionDefinition("DAILY_COLLECTION_SUMMARY",
                                                                                        typeof(CollectionSummaryReportDAL),
                                                                                        "GetDailyCollectionData",
                                                                                        "GetDailyCollectionSummary",
                                                                                        typeof(DailyCollectionSummaryFilter),
                                                                                        typeof(DailyCollectionDataRow))
                        };

        public static ReportExecutionDefinition? Get(string reportCode)
        {
            return _reports.TryGetValue(reportCode, out var definition) ? definition : null;
        }
    }

}