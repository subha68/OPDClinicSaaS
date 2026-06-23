using Helpers;

namespace Models
{
    public class Bill
    {
        public int BillId { get; set; }
        public string BillNo { get; set; }

        public DateTime BillDate { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public Patient? Patient { get; set; }

        public int VisitId { get; set; }
        public Visit? Visit { get; set; }
        public DateTime? VisitDate { get; set; }

        public int ClinicId { get; set; }
        public Clinic? Clinic { get; set; }
        public string BillType { get; set;}

        public decimal GrossAmount { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TaxableAmount => GrossAmount - DiscountAmount;
        public decimal TaxAmount { get; set; } = 0;
        public decimal CGST { get; set; } = 0;
        public decimal SGST { get; set; } = 0;
        public decimal IGST { get; set; } = 0;
        public decimal NetAmount => TaxableAmount + TaxAmount;
        public string? AmountInWords { 
                        get
                        {
                            return NumberToWords.Convert(NetAmount);
                        }
                    }
        public string? Remarks { get; set; }

        public List<BillLine>? BillLines { get; set; } = new List<BillLine>();

        public decimal TotalPaidAmount { get; set; } = 0;
        public string PaymentStatus { 
                            get
                            {
                                if (TotalPaidAmount > 0 && TotalPaidAmount < NetAmount)
                                    return "Partially Paid";
                                else if (TotalPaidAmount >= NetAmount)
                                    return "Paid";
                                else
                                    return "Due";
                            }
                        }
        public decimal BalanceAmount { 
                        get
                        {
                            return NetAmount - TotalPaidAmount;
                        }
                    }
        public List<Payment>? Payments { get; set; } = new List<Payment>();      
    }

}