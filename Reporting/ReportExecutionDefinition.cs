namespace BLL
{
    public class ReportExecutionDefinition
    {
        public string ReportCode { get; set; } = "";
        public Type DalType { get; set; } = null!;
        public string MethodName { get; set; } = "";
        public Type FilterType { get; set; } = null!;
        public Type RowType { get; set; } = null!;
        public string SummaryMethodName { get; set; } = "";


        public ReportExecutionDefinition(string reportCode, Type dalType, string methodName, string summaryMethodName, Type filterType, Type rowType)
        {
            ReportCode = reportCode;
            DalType = dalType;
            MethodName = methodName;
            SummaryMethodName = summaryMethodName;
            FilterType = filterType;
            RowType = rowType;
        }
    }
}