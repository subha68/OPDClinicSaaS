using Models;

namespace ViewModels
{
    public class TestResultGroupVM
    {
        public int TestResultGroupId { get; set; }
        public int ClinicId { get; set; }
        public int PatientId { get; set; }
        public string? PatientCode { get; set; }
        public string? PatientName { get; set; }
        public int? VisitId { get; set; }
        public DateTime? VisitDate { get; set; }
        public List<Visit>? Visits { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime? ResultDate { get; set; }
        public string? LabName { get; set; }
        public string? ReportNo { get; set; }
        public string? Remarks { get; set; }
        public string ParameterSummary { get; set; } = string.Empty;
        public List<TestResultVM> Results { get; set; } = new();
        public List<TestResultVM>? PreviousResults { get; set; } = new();

        public TestResultGroupVM()
        {
            // default constructor
        }

        public TestResultGroupVM(TestResultGroup model)
        {
            TestResultGroupId = model.TestResultGroupId;
            ClinicId = model.ClinicId;
            PatientId = model.PatientId;
          //  PatientCode
          //  PatientName
            VisitId = model.VisitId;
            VisitDate = model.VisitDate;
            TestDate = model.TestDate;
            ResultDate = model.ResultDate;
            LabName = model.LabName;
            ReportNo = model.ReportNo;
            Remarks = model.Remarks;
            ParameterSummary = model.ParameterSummary;

            Results = new List<TestResultVM>();

            if (model.Results != null && model.Results.Count > 0)
            {
                foreach (var result in model.Results)
                {
                    Results.Add(new TestResultVM(result));
                }
            }
            PreviousResults = new List<TestResultVM>();
        }
    }
}