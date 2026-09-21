namespace Models
{
    public class ConsultationTestResultGroup
    {
        public int ConsultationTestResultGroupId { get; set; }
        public int ClinicId { get; set; }
        public int PatientId { get; set; }
        public int VisitId { get; set; }
        public int RowId { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime VisitDate { get; set; }

        public List<ConsultationTestResult> TestResults { get; set; } = [];

        public ConsultationTestResultGroup()
        {
            // default constructor
        }

        public ConsultationTestResultGroup(TestResultGroup group)
        {
            ClinicId = group.ClinicId;
            PatientId = group.PatientId;
            VisitId = group.VisitId ?? 0;
            TestDate = group.TestDate;

        }
    }
}