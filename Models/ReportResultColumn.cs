namespace Models
{
    public class ReportResultColumn
    {
        public string ColumnCode { get; set; } = "";

        public string DisplayLabel { get; set; } = "";

        public string DataType { get; set; } = "";

        public int DisplayOrder { get; set; }

        public string? FormatPattern { get; set; }
    }
}