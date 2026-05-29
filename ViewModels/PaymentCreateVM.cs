using Models;

namespace ViewModels
{
    public class PaymentCreateVM
    {
        // Bill Information
        public Bill Bill { get; set; }

        // New Payment Entry
        public Payment Payment { get; set; } = new();
    }
}