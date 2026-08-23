namespace Models
{
    public class DailyCollectionDataFilter
    {
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public string? PaymentMode { get; set; }
        public int ClinicId { get; set; }
    }
}