namespace Models
{
    public class Journal
    {
        public int JournalHeaderId { get; set; }
        public int ClinicId { get; set; }
        public DateTime JournalDate { get; set; }
        public string VoucherType { get; set; }
        public string ReferenceType { get; set; }
        public int ReferenceId { get; set; }
        public string Narration { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<JournalLine> Lines { get; set; }

        public Journal()
        {
        }

        public Journal(Bill bill)
        {
            ClinicId = bill.ClinicId;
            JournalDate = bill.BillDate;
            ReferenceId = bill.BillId;
            ReferenceType = bill.BillType;
            VoucherType = "Bill";
            Narration = "Patient ID - " + bill.Patient.PatientId + ", Name - " + bill.Patient.FirstName + ' ' + bill.Patient.LastName;

            Lines = new List<JournalLine>();
        }

        public Journal(Refund refund)
        {
            ClinicId = refund.ClinicId;
            JournalDate = DateTime.Now;
            VoucherType = "Refund";
            ReferenceType = "Refund";
            ReferenceId = refund.RefundId;
            Narration = refund.RefundStatus == RefundStatus.PendingApproval 
                                                ? $"Refund Approval #{refund.RefundId}" 
                                                : $"Refund Payment #{refund.RefundId}";
        
            Lines = new List<JournalLine>();
        }

        public Journal(Payment payment)
        {
            ClinicId = payment.ClinicId;
            JournalDate = payment.PaymentDate;
            ReferenceId = payment.PaymentId;
            ReferenceType = "Payment";
            VoucherType = "Payment";
            Narration = "Patient ID - " + payment.Patient.PatientId + ", Name - " + payment.Patient.FirstName + ' ' + payment.Patient.LastName;

            Lines = new List<JournalLine>();
        }
    }

}