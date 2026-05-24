using System;
using System.Collections.Generic;

namespace Models
{
    public class Consultation
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }

        public string? Symptoms { get; set; }

        public string? Diagnosis { get; set; }
        public List<PharmacologyAdvice> pharmacologyAdvices { get; set; } = new();

        public string? Notes { get; set; }
    }
}