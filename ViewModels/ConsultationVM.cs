using System;
using System.Collections.Generic;

namespace ViewModels
{
    public class ConsultationVM
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }

        public string PatientName { get; set; }

        public string? Symptoms { get; set; }

        public string? Diagnosis { get; set; }
        public List<PharmacologyAdviceVM> pharmacologyAdvices { get; set; } = new();

        public string? Notes { get; set; }
    }
}