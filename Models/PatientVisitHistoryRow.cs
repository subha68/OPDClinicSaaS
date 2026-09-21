namespace Models
{
    public class PatientVisitHistoryRow
    {
        public DateTime VisitDate { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string VisitType { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}