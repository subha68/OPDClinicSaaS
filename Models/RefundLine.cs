namespace Models
{
    public class RefundLine
    {
        public int RefundLineId { get; set; }
        public int RefundId { get; set; }
        public int RefundLineNo { get; set; }
        public int BillLineId { get; set; }
        public int BillLineNo { get; set; }
        public ServiceType? ServiceType { get; set; }
        public string? Description { get; set; }
        public decimal Rate { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal GrossAmount => Rate * Quantity;
        public decimal DiscountPercent { get; set; } = 0;
        public decimal DiscountAmount => GrossAmount * DiscountPercent / 100;
        public decimal TaxableAmount => GrossAmount - DiscountAmount;
        public decimal TaxPercent { get; set; } = 0;
        public decimal TaxAmount => TaxableAmount * TaxPercent / 100;
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal NetAmount => TaxableAmount + TaxAmount;

        public RefundLine()
        {
            // default constructor
        }

        public RefundLine(BillLine billLine)
        {
            RefundLineId = 0;
            RefundId = 0;
            RefundLineNo = 0;
            BillLineId = billLine.LineId;
            ServiceType = (ServiceType)billLine.ServiceType;
            Description = billLine.Description;
            Quantity = billLine.AvailableQuantity;
            Rate = billLine.Rate;
            DiscountPercent = billLine.DiscountPercent;
            TaxPercent = billLine.TaxPercent;
            CGST = billLine.CGST;
            SGST = billLine.SGST;
            IGST = billLine.IGST;
        }
    }
}