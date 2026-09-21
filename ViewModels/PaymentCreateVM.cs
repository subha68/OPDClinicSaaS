using Models;

namespace ViewModels
{
    public class PaymentCreateVM
    {
        // Bill Information
        public Bill Bill { get; set; } = new();

        // New Payment Entry
        public Payment Payment { get; set; } = new();

        public PaymentCreateVM()
        {
            
        }

        public PaymentCreateVM(Bill bill)
        {
            Bill = bill;
            Payment = new Payment(bill);
        }
    }
}