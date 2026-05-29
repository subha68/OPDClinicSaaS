using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class BackButtonVM
    {
        public string Controller { get; set; }
        public string Action { get; set; }

        public int? Id { get; set; }
        public int? PatientId { get; set; }

        public string Text { get; set; } = "Back";
    }
}