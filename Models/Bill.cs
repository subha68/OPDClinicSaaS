namespace Models
{
    public class Bill
    {
        public int BillId { get; set; }

        public DateTime BillDate { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int VisitId { get; set; }
        public Visit? Visit { get; set; }

        public int ClinicId { get; set; }
        public Clinic? Clinic { get; set; }

        public decimal? GrossAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? TotalPaidAmount { get; set; } = 0;
        public string Status { get; set; } = "Due";
        public decimal? BalanceAmount { get; set; }
        public ICollection<Payment>? Payments { get; set; }        

        public string? AmountInWords { get; set; }

        public ICollection<BillLine> BillLines { get; set; }
    }

}