namespace Models
{
    public class PharmacyBill
    {
        public int PharmacyBillId { get; set; }
        public string PharmacyBillNo { get; set; }
        public int PatientId { get; set; }
        public int PharmacologyId { get; set; }
        public DateTime BillDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal NetAmount {
            get
            {
                return GrossAmount - Discount + Tax;
            } 
        }
        public string? Remarks { get; set; }

    }
}