using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class BackButtonVM
    {
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;

        public int? Id { get; set; }
        public int? PatientId { get; set; }

        public string Text { get; set; } = "Back";
    }
}