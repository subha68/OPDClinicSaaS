using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class BreadcrumbItemVM
    {
        public string Text { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public int? Id { get; set; }

        public int? PatientId { get; set; }
    }
}