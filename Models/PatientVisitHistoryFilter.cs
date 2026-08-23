namespace Models
{
    public class PatientVisitHistoryFilter
    {
        public int PatientId { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
        public int ClinicId { get; set; }
    }
}