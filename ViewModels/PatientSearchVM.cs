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
        public int? DepartmentId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime? AppointmentDateTime { get; set; }
        public DateTime? AppointmentDate
        {
            get
            {
                return AppointmentDateTime?.Date;
            }
        }

        public string? SlotTime 
        { 
            get
            {
                return AppointmentDateTime?.ToString("HH:mm");
            }
        }
        public DateTime? LastVisitDate { get; set; }

        public List<Patient> Results { get; set; } = new();
    }
}