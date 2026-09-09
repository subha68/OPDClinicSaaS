using Models;
using Helpers;

namespace ViewModels
{
    public class TestResultVM
    {
        public DateTime VisitDate { get; set; }
        public int TestResultId { get; set; }
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
        public string? PreviousValue { get; set; }
        public DateTime? PreviousResultDate { get; set; }
        public string? ResultFlag { get; set; }

        // UI-only support

        public List<LookupItem> AvailablePanels { get; set; } = new();
        public List<LookupItem> ResultFlags { get; set; } = EnumHelper.ToLookup<ResultFlag>();

        public TestResultVM()
        {
            // Default constructor
        }

        public TestResultVM(TestResult model)
        {
            TestResultId = model.TestResultId;
            TestPanelId = model.TestPanelId;
            TestPanelCode = model.TestPanelCode;
            TestPanelName = model.TestPanelName;
            TestParameterId = model.TestParameterId;
            TestParameterCode = model.TestParameterCode;
            TestParameterName = model.TestParameterName;
            PanelDisplayOrder = model.PanelDisplayOrder;
            ParameterDisplayOrder = model.ParameterDisplayOrder;
            VisitDate = model.VisitDate;
            NormalRange = model.NormalRange;
            NormalMin = model.NormalMin;
            NormalMax = model.NormalMax;
            TestMethod = model.TestMethod;
            DataType = model.DataType;
            Value = model.Value;
            Unit = model.Unit;
            // PreviousValue
            // PreviousResultDate
            ResultFlag = model.ResultFlag;
        }
    }
}