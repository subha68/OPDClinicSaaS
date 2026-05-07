using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public string FirstName { get; set; }
    
        [Required]
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    
        public int? ClinicId { get; set; }

        public bool IsFirstLogin { get; set; }
        public bool IsActive { get; set; }
    }

}