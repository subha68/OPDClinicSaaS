using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }       // doctors must be associated with a clinic
        public string? ClinicName { get; set; }
        public int UserId { get; set; }

        public string Salutation { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } 
        public string DoctorName
        {
            get
            {
                return Salutation + " " + FirstName + " " + LastName;
            }
        }

        public int DepartmentId { get; set; }
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        public string RegistrationNo { get; set; }
        public string Qualification { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public decimal Fee { get; set; }
        public int? PrescriptionTemplateId { get; set; }
        public string? PrescriptionTemplateName { get; set; }
        public int? SourceTemplateId { get; set; }

    }
}
