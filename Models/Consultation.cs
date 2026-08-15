using System;
using System.Collections.Generic;

namespace Models
{
    public class Consultation
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan SlotStartTime { get; set; }
        public int ClinicId { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime? DOB { get; set; }
        public int Age
        {
            get
            {
                if (DOB == null || DOB == DateTime.MinValue)
                    return 0;

                var today = DateTime.Today;

                int age = today.Year - DOB.Value.Year;

                if (DOB.Value.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }
        public string Gender { get; set; }
        public ConsultationStatus Status { get; set; }
        public bool IsDraft => Status == ConsultationStatus.Draft;
        public bool IsCompleted => Status == ConsultationStatus.Completed;

        public List<ConsultationSection> Sections { get; set; }

        public int CreatedBy { get; set; }

        public Consultation()
        {
            Sections = new List<ConsultationSection>();
        }

        public Consultation(PrescriptionTemplate template, 
                                    int appointmentId,
                                    DateTime appointmentDate,
                                    TimeSpan slotStartTime, 
                                    int doctorId, 
                                    string doctorName, 
                                    int patientId, 
                                    string patientName, 
                                    DateTime? dob,
                                    string gender,
                                    int clinicId)
        {
            AppointmentId = appointmentId;
            AppointmentDate = appointmentDate;
            SlotStartTime = slotStartTime;
            ClinicId = clinicId;
            TemplateId = template.TemplateId;
            TemplateName = template.TemplateName;
            DoctorId = doctorId;
            DoctorName = doctorName;
            PatientId = patientId;
            PatientName = patientName;
            DOB = dob;
            Gender = gender;
            Status = ConsultationStatus.Draft; // Default status

            Sections = new List<ConsultationSection>();
            if (template.Sections != null)
            {
                foreach (var templateSection in template.Sections)
                {
                    ConsultationSection section = new ConsultationSection(templateSection, clinicId);
                    Sections.Add(section);
                }
            }
        }
    }
}