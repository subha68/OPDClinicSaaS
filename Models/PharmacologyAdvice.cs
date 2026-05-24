using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models
{
    public class PharmacologyAdvice
    {
        public int PrescriptionId { get; set; }
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }

        public string? MedicineName { get; set; }
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
        public string? Instructions { get; set; }
    }
}