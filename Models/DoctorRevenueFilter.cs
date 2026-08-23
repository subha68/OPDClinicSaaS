namespace Models
{
    public class DoctorRevenueFilter
    {
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public int? DoctorId { get; set; }
        public int ClinicId { get; set; } 
    }
}