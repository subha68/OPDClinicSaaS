namespace Models
{
    public class DoctorAppointmentSummaryRow
    {
        public string DoctorName { get; set; } = "";
        public int AppointmentCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int NoShowCount { get; set; }
    }
}