using Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ViewModels
{
    public class PaymentCreateVM
    {
        // ------------------------------------------------------------
        // SOURCE DOCUMENT
        // ------------------------------------------------------------

        [ValidateNever]
        public Bill Bill { get; set; } = new();

        [ValidateNever]
        public Refund Refund { get; set; } = new();

        // ------------------------------------------------------------
        // PAYMENT ENTRY
        // ------------------------------------------------------------
        public Payment Payment { get; set; } = new();

        // ------------------------------------------------------------
        // DISPLAY
        // ------------------------------------------------------------
        public decimal OutstandingAmount { get; set; }
        public bool IsRefund =>
            Payment?.PaymentType == PaymentType.Refund;
        public bool IsReceipt =>
            Payment?.PaymentType == PaymentType.Receipt;
        public string TransactionTitle =>
            IsRefund
                ? "Make Refund Payment"
                : "Receive Payment";
        public string PaymentActionText =>
            IsRefund
                ? "Make Payment"
                : "Receive Payment";
        public string HistoryTitle =>
            IsRefund
                ? "Refund Payment History"
                : "Payment History";

        // ------------------------------------------------------------
        // DEFAULT CONSTRUCTOR
        // ------------------------------------------------------------

        public PaymentCreateVM()
        {
        }

        // ------------------------------------------------------------
        // RECEIPT
        // ------------------------------------------------------------

        public PaymentCreateVM(Bill bill)
        {
            Bill = bill;
            Payment = new Payment(bill);
            OutstandingAmount = Math.Max(0, bill.NetAmount - bill.TotalPaidAmount);
        }

        // ------------------------------------------------------------
        // REFUND PAYMENT
        // ------------------------------------------------------------

        public PaymentCreateVM(Refund refund)
        {
            Refund = refund;
            Payment = new Payment(refund);
            OutstandingAmount = Math.Max(0, refund.NetAmount - refund.RefundedAmount);
        }
    }
}