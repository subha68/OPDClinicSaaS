using Models;

namespace ViewModels
{
    public class TestPanelVM
    {
        public TestPanel Panel { get; set; } = new();

        public List<TestParameterVM> Parameters { get; set; } = new();
    }

}