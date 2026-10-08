using Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Models
{
    public class Refund
    {
        public int RefundId { get; set; }

        [ValidateNever]
        public string RefundNo { get; set; } = "";
        public int BillId { get; set; }
        public int ClinicId { get; set; }
        public Clinic? Clinic { get; set; }
        public int? VisitId { get; set; }
        public int? PatientId { get; set; }
        public DateTime RefundDate { get; set; }
        public RefundStatus RefundStatus { get; set; } = RefundStatus.Draft;
        public decimal GrossAmount { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TaxableAmount => GrossAmount - DiscountAmount;
        public decimal TaxAmount { get; set; } = 0;
        public decimal CGST { get; set; } = 0;
        public decimal SGST { get; set; } = 0;
        public decimal IGST { get; set; } = 0;
        public decimal NetAmount => TaxableAmount + TaxAmount;
        public decimal RefundedAmount { get; set; } = 0;
        public decimal BalanceAmount => NetAmount - RefundedAmount;

        [ValidateNever]
        public Bill Bill { get; set; } = new();   // display only property
        public List<Payment> Payments { get; set; } = [];
        public string? AmountInWords { 
                        get
                        {
                            return NumberToWords.Convert(NetAmount);
                        }
                    }
        public string? Reason { get; set; }
        public int? RequestedBy { get; set; }
        public DateTime? RequestedOn { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string? RejectionReason { get; set; }
        public int? CompletedBy { get; set; }
        public DateTime? CompletedOn { get; set; }
        public List<RefundLine> Lines { get; set; } = new();
        // Display only properties
        public string? PatientName { get; set; }
        public string? BillNo { get; set; }
        public string? RequestedByName { get; set; }
        public string? ApprovedByName { get; set; }
        public string? CompletedByName { get; set; }
    }
}