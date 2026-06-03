using Models;

namespace ViewModels
{
    public class PatientSearchVM
    {
        public int ClinicId { get; set; }

        public string? SearchString { get; set; }

        // AppointmentBooking
        // Consultation
        // Billing
        // PatientHistory
        public string? ReturnTo { get; set; }

        // Optional values required only by some flows
        public int? DoctorId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public int? SlotId { get; set; }
        public string? SlotTime { get; set; }

        public List<Patient> Results { get; set; } = new();
    }
}