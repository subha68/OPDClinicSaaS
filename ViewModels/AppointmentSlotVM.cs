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
    }
}