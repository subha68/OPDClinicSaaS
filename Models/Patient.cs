using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }
    
        [Required]
        public string LastName { get; set; }
        public string FullName => FirstName + ' ' + LastName; 

        [Required]
        public DateTime? DOB { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Required]
        public string? Phone { get; set; }    
        public int? ClinicId { get; set; }

        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }

        public string? IdType { get; set; }
        public string? IdNumber { get; set; }

        public string? EmergencyContactName { get; set; }
        public string? EmergencyRelationship { get; set; }
        public string? EmergencyPhone { get; set; }


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

        public int DisplayAge { get; set; }

        public int? LastVisitId { get; set; }
        public DateTime? LastVisitDate { get; set; }

    }

}