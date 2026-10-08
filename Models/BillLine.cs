namespace Models
{
    public class BillLine
    {
        public int LineId { get; set; }

        public int BillId { get; set; }
        public int PatientId { get; set; }

        // Charge based - Registration, Consultation, Lab Tests, Procedures, Vaccinations, Radiology
        // Medicine based - Pharmacy
        public ServiceType ServiceType { get; set; }          
        public int ReferenceId { get; set; }  // ChargeId, MedicineBatchId
        public string ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }

        public int Quantity { get; set; } = 1;
        public int AvailableQuantity { get; set; }
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

        public BillLine()
        {
            // default constructor
            AvailableQuantity = Quantity;
        }
    }
}