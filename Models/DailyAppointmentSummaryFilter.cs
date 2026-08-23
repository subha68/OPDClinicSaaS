namespace Models
{
    public class DailyAppointmentSummaryFilter
    {
        public DateOnly AppointmentDate { get; set; }

        public int? DoctorId { get; set; }
        public int ClinicId { get; set; }
    }
}