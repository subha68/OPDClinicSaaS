namespace Models
{
    public class ConsultationTestResult
    {
        public int ConsultationTestResultId { get; set; }
        public int ConsultationTestResultGroupId { get; set; }
        public int ClinicId { get; set; }
        public int TestResultId { get; set; }
        public int TestResultGroupId { get; set; }
        public int TestPanelId { get; set; }
        public string TestPanelCode { get; set; } = string.Empty;
        public string TestPanelName { get; set; } = string.Empty;
        public int TestParameterId { get; set; }
        public string TestParameterCode { get; set; } = string.Empty;
        public string TestParameterName { get; set; } = string.Empty;
        public int PanelDisplayOrder { get; set; }
        public int ParameterDisplayOrder { get; set; }
        public string? NormalRange { get; set; }
        public decimal? NormalMin { get; set; }
        public decimal? NormalMax { get; set; }
        public string? TestMethod { get; set; }
        public string DataType { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? Unit { get; set; }
        public string? ResultFlag { get; set; }

        // Navigation Properties

        public ConsultationRow? Row { get; set; }
        public TestResult? TestResult { get; set; }

        public ConsultationTestResult()
        {
            // default constructor
        }

        public ConsultationTestResult(TestResult result)
        {
            ClinicId = result.ClinicId;
            TestResultId = result.TestResultId;
            TestResultGroupId = result.TestResultGroupId;
            TestPanelId = result.TestPanelId;
            TestPanelCode = result.TestPanelCode;
            TestPanelName = result.TestPanelName;
            TestParameterId = result.TestParameterId;
            TestParameterCode = result.TestParameterCode;
            TestParameterName = result.TestParameterName;
            PanelDisplayOrder = result.PanelDisplayOrder;
            ParameterDisplayOrder = result.ParameterDisplayOrder;
            NormalRange = result.NormalRange;
            NormalMin = result.NormalMin;
            NormalMax = result.NormalMax;
            TestMethod = result.TestMethod;
            DataType = result.DataType;
            Value = result.Value;
            Unit = result.Unit;
            ResultFlag = result.ResultFlag;
        }
    }
}