using System;

namespace Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int? BillId { get; set; }
        public int? RefundId { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int ClinicId { get; set; }
        public PaymentType PaymentType { get; set; } = PaymentType.Receipt;
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Mode { get; set; } = string.Empty;
        public string? ReferenceNo { get; set; }
        public DateTime CreatedAt { get; set; }

        public Payment()
        {}

        public Payment(Bill bill)
        {
            BillId = bill.BillId;
            PatientId = bill.PatientId;
            ClinicId = bill.ClinicId;
            PaymentDate = DateTime.Today;
            PaymentType = PaymentType.Receipt;
            Amount = bill.BalanceAmount;
        }

        public Payment(Refund refund)
        {
            RefundId = refund.RefundId;
            PatientId = refund.PatientId ?? 0;
            ClinicId = refund.ClinicId;
            PaymentDate = DateTime.Today;
            PaymentType = PaymentType.Refund;
            Amount = refund.NetAmount - refund.RefundedAmount;
        }
    }
}