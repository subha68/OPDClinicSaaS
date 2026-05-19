using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class PrescriptionVM
    {
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; }
        public string Qualification { get; set; }
        public string RegistrationNumber { get; set; }

        public string ClinicName { get; set; }
        public string ClinicAddress { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string PatientPhoneNo { get; set; }

        public DateTime VisitDateTime { get; set; }

        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }

        public List<PharmacologyAdviceVM> Medicines { get; set; }    
    }
}