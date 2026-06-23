using System;

namespace Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int BillId { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int ClinicId { get; set; }
        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }

        public string Mode { get; set; }

        public string ReferenceNo { get; set; }

        public DateTime CreatedAt { get; set; }

        public Payment()
        {}

        public Payment(Bill bill)
        {
            BillId = bill.BillId;
            PaymentDate = DateTime.Today;
            Amount = bill.BalanceAmount;
        }
    }
}