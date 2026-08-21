namespace Models
{
    public class ReportColumn
    {
        public long ReportColumnId { get; set; }
        public long ReportId { get; set; }
        public string ColumnCode { get; set; } = "";
        public string DisplayLabel { get; set; } = "";
        public string DataType { get; set; } = "";
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public string? FormatPattern { get; set; }
    }
}