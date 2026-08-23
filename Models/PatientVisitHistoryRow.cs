namespace Models
{
    public class PatientVisitHistoryRow
    {
        public DateTime VisitDate { get; set; }
        public string DoctorName { get; set; }
        public string VisitType { get; set; }
        public string Diagnosis { get; set; }
        public decimal Amount { get; set; }
    }
}