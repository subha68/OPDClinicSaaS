using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace ViewModels
{
    public class BillListVM
    {
        public int BillId { get; set; }

        public DateTime VisitDate { get; set; }

        public string DoctorName { get; set; }

        public int PatientId { get; set; }

        public string PatientName { get; set; }

        public DateTime BillDate { get; set; }

        public decimal BillAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public string Status { get; set; }   // Paid / Due
    }
}