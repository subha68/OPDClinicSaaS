using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class LoginVM
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public int? ClinicId { get; set; }
        public string? ClinicCode { get; set; }
    }
}