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
        public int ConsultationId { get; set; }

        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }

        public List<PharmacologyAdvice> Medicines { get; set; } 

        public PrescriptionVM()
        {
        }

    /*    public PrescriptionVM(Appointment appointment, 
                                Visit visit, 
                                Consultation consultation, 
                                Patient patient, 
                                List<PharmacologyAdvice> medicines)
        {
            AppointmentId = appointment.AppointmentId;
            DoctorName = appointment.DoctorName;
            Qualification = appointment.Qualification;
            RegistrationNumber = appointment.RegistrationNumber;

            ClinicName = appointment.ClinicName;
            ClinicAddress = appointment.ClinicAddress;

            PatientId = patient.PatientId;
            PatientName = $"{patient.FirstName} {patient.LastName}";
            PatientPhoneNo = patient.Phone;
            Gender = patient.Gender;
            Age = patient.Age;
            VisitDateTime = visit.VisitDate;
            Symptoms = consultation.Symptoms;
            Diagnosis = consultation.Diagnosis;
            Notes = consultation.Notes;
            Medicines = medicines;
        } */
    }
}