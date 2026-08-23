namespace Models
{
    public class PatientRegistrationSummaryFilter
    {
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public int ClinicId { get; set; }
    }
}