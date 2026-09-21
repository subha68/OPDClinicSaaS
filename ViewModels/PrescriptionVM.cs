using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class PrescriptionVM
    {
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;

        public string ClinicName { get; set; } = string.Empty;
        public string ClinicAddress { get; set; } = string.Empty;
        public string ClinicLogo { get; set; } = string.Empty;
        public string ClinicPhoneNo { get; set; } = string.Empty;

        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PatientPhoneNo { get; set; } = string.Empty;
        public int VisitId { get; set; }
        public DateTime VisitDate { get; set; }
        public DateTime VisitDateTime { get; set; }
        public int ConsultationId { get; set; }

        public List<PrescriptionSectionVM> Sections { get; set; }
        = new List<PrescriptionSectionVM>(); 

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