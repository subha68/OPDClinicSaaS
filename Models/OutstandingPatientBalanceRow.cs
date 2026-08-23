namespace Models
{
    public class OutstandingPatientBalanceRow
    {
        public string PatientNumber { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string InvoiceNumber { get; set; } = "";
        public DateTime InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
    }
}