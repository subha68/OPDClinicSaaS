namespace Models
{
    public class PharmacyBillLine
    {
        public int PBLineId { get; set; }
        public int PharmacyBillId { get; set; }
        public int PBLineNo { get; set; }
        public int MedicineId { get; set; }
        public int BatchId { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal GrossAmount { 
            get
            {
                return Quantity * Rate;
            } 
        }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { 
            get
            {
                return GrossAmount * DiscountPercent / 100;
            } 
        }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { 
            get
            {
                return (GrossAmount - DiscountAmount) * TaxPercent / 100;
            } 
        }
        public decimal NetAmount { 
            get
            {
                return GrossAmount - DiscountAmount + TaxAmount;
            }
        }
    }

}