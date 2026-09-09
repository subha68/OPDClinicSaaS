using Models;

namespace ViewModels
{
    public class TestParameterVM
    {
        public TestParameter Parameter { get; set; } = new();
        public TestPanelParameter Association { get; set; } = new();
    }

}