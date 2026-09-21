using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class BreadcrumbItemVM
    {
        public string Text { get; set; } = string.Empty;

        public string Controller { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public int? Id { get; set; }

        public int? PatientId { get; set; }
    }
}