using System;

namespace Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int DoctorId { get; set; }

        public int ClinicId { get; set; }

        public int PatientId { get; set; }
        public int VisitId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public TimeSpan SlotTime { get; set; }

        public string Status { get; set; }  // Booked, In Consultation, Completed, Cancelled, No Show 

        public string? ClinicName { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        
    }
}