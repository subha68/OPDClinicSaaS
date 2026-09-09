using ViewModels;

namespace Models
{
    public class TestResultGroup
    {
        public int TestResultGroupId { get; set; }
        public int ClinicId { get; set; }
        public int PatientId { get; set; }
        public int? VisitId { get; set; }
        public DateTime? VisitDate { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime? ResultDate { get; set; }
        public string? LabName { get; set; }
        public string? ReportNo { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation Properties
        public Patient? Patient { get; set; }
        public Visit? Visit { get; set; }
        public string ParameterSummary { get; set; } = string.Empty;
        public List<TestResult> Results { get; set; } = new();

        public TestResultGroup()
        {
            // default constructor
        }

        public TestResultGroup(TestResultGroupVM vm)
        {
            TestResultGroupId = vm.TestResultGroupId;
            ClinicId = vm.ClinicId;
            PatientId = vm.PatientId;
            VisitId = vm.VisitId;
            TestDate = vm.TestDate;
            ResultDate = vm.ResultDate;
            LabName = vm.LabName;
            ReportNo = vm.ReportNo;
            Remarks = vm.Remarks;
         //   CreatedDate
        }
    }
}
