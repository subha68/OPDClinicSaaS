using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Visit
    {
        public int VisitId { get; set; }
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; }
        public int ClinicId { get; set; }
        public int PatientId { get; set; }
    }
}


