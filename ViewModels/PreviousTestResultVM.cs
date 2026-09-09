using Models;

namespace ViewModels
{
    public class PreviousTestResultVM
    {
        public DateTime TestDate { get; set; }

        public string? Value { get; set; }

        public string? Unit { get; set; }

        public string? ResultFlag { get; set; }
    }

}