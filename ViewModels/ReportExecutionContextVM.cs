namespace ViewModels
{
    public class ReportExecutionContextVM
    {
        public ReportDefinitionVM Definition { get; set; } = null!;
        public Dictionary<string, object?> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public int ClinicId { get; set; }
    }
}