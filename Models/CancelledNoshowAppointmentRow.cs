namespace Models
{
    public class CancelledNoshowAppointmentRow
    {
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string AppointmentStatus { get; set; }
        public string? Remarks { get; set; }
    }
}