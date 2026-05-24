using Models;

namespace ViewModels
{
    public class PatientHistoryVM
    {
        public Patient Patient { get; set; }

        public List<Appointment>? Appointments { get; set; }
        public List<Consultation>? Consultations { get; set; }
        public List<PrescriptionVM>? Prescriptions { get; set; }
    }
}