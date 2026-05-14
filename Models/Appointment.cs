using System;

namespace Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int DoctorId { get; set; }

        public int ClinicId { get; set; }

        public int PatientId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public DateTime SlotTime { get; set; }

        public string Status { get; set; }
        
    }
}