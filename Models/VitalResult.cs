namespace Models
{
    public class VitalResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<Vital> Vitals { get; set; } = new();
    }
}
