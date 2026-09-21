using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;
    
        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }
    
        public int? ClinicId { get; set; }
        public string? ClinicName { get; set; }

        public bool IsFirstLogin { get; set; }
        public bool IsActive { get; set; }
    }

}