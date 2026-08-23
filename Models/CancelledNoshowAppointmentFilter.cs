namespace Models
{
    public class CancelledNoshowAppointmentFilter
    {
        public DateOnly DateFrom { get; set; } 
        public DateOnly DateTo { get; set; }
        public int? DoctorId { get; set; }
        public string? AppointmentStatus { get; set; }
        public int ClinicId { get; set; }
    }
}