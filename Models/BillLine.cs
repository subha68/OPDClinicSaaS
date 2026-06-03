namespace Models
{
    public class BillLine
    {
        public int LineId { get; set; }

        public int BillId { get; set; }
        public int PatientId { get; set; }

        public int ChargeId { get; set; }
        public string ChargeCode { get; set; }
        public string ChargeDescription { get; set; }
        public BillingCharge? BillingCharge { get; set; }

        public decimal Rate { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal GrossAmount => Rate * Quantity;

        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount => GrossAmount * DiscountPercent / 100;
        public decimal NetAmount => GrossAmount - DiscountAmount;
    }
}