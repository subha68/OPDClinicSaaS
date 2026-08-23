namespace Models
{
    public class DoctorRevenueRow
    {
        public string DoctorName { get; set; }
        public int ConsultationCount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
    }
}