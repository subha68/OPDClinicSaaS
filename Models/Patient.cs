using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required]
        public string? FirstName { get; set; }
    
        [Required]
        public string? LastName { get; set; }

        [Required]
        public DateTime? DOB { get; set; }
        [Required]
        public string? Gender { get; set; }

        public string? Address { get; set; }

        [Required]
        public string? Phone { get; set; }
    
        public int? ClinicId { get; set; }

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

    }

}