namespace Models
{
    public class ReportSummaryItem
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public ReportSummaryItem(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }
}