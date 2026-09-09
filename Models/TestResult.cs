namespace Models
{
    public class TestResult
    {
        public int TestResultId { get; set; }
        public int TestResultGroupId { get; set; }
        public int ClinicId { get; set; }
        public DateTime VisitDate { get; set; }
        public DateTime TestDate { get; set; }
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

        // Navigation Property
        public TestResultGroup? Group { get; set; }

        public TestResult()
        {
            // default constructor
        }

        public TestResult(TestResult model)
        {
            // copy constructor
            TestResultId = model.TestResultId;
            TestResultGroupId = model.TestResultGroupId;
            ClinicId = model.ClinicId;
            VisitDate = model.VisitDate;
            TestDate = model.TestDate;
            TestPanelId = model.TestPanelId;
            TestPanelCode = model.TestPanelCode;
            TestPanelName = model.TestPanelName;
            TestParameterId = model.TestParameterId;
            TestParameterCode = model.TestParameterCode;
            TestParameterName = model.TestParameterName;
            PanelDisplayOrder = model.PanelDisplayOrder;
            ParameterDisplayOrder = model.ParameterDisplayOrder;
            NormalRange = model.NormalRange;
            NormalMin = model.NormalMin;
            NormalMax = model.NormalMax;
            TestMethod = model.TestMethod;
            DataType = model.DataType;
            Value = model.Value;
            Unit = model.Unit;
            ResultFlag = model.ResultFlag;
        }
    }
}
