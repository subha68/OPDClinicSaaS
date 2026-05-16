using System;

namespace ViewModels
{
    public class AppointmentVM
    {
        public int AppointmentId { get; set; }

        public int DoctorId { get; set; }

        public int ClinicId { get; set; }

        public int PatientId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public TimeSpan SlotTime { get; set; }

        public string Status { get; set; }  // Booked, In Consultation, Completed, Cancelled, No Show 

        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        
    }
}