using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required]
        public string FirstName { get; set; }
    
        [Required]
        public string LastName { get; set; }

        [Required]
        public string DOB { get; set; }

        [Required]
        public string Gender { get; set; }

        public string? Address { get; set; }

        public string Phone { get; set; }
    
        public int? ClinicId { get; set; }

    }

}