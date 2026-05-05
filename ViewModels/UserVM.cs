using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class UserVM
    {
        [Required]
        public User User { get; set; }

        public List<SelectListItem>? Roles { get; set; }

        public List<SelectListItem>? Clinics { get; set; }
    }
}