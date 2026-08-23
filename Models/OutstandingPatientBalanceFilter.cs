namespace Models
{
    public class OutstandingPatientBalanceFilter
    {
        public DateOnly AsOfDate { get; set; }
        public int? PatientId { get; set; }
        public int ClinicId { get; set; }
    }
}