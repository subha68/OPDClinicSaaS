using Models;

namespace ViewModels
{
    public class AppointmentSlotVM
    {
        public string SlotTime { get; set; }
        public bool IsBooked { get; set; }
        public int? AppointmentId { get; set; }
        public int? PatientId { get; set; }
        public string PatientCode { get; set; }
        public string PatientName { get; set; }
        public string AppointmentStatus { get; set; }
        public string BillingStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string SlotStatus { get; set; }
        public int? TokenNumber { get; set; }
        public int? QueueNumber { get; set; }        

        public List<Appointment> CancelledAppointments { get; set; } = new();
    }
}