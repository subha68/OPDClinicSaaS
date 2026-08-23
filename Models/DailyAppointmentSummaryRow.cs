namespace Models
{
    public class DailyAppointmentSummaryRow
    {
        public TimeSpan? AppointmentTime { get; set; }
        public string PatientName { get; set; } = "";
        public string PatientNumber { get; set; } = "";
        public string DoctorName { get; set; } = "";
        public string AppointmentType { get; set; } = "";
        public string AppointmentStatus { get; set; } = "";
    }
}