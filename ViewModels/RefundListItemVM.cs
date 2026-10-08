using Models;

namespace ViewModels
{
    public class RefundListItemVM
    {
        public int RefundId { get; set; }
        public string RefundNo { get; set; } = "";
        public int BillId { get; set; }
        public string? BillNo { get; set; }
        public DateTime RefundDate { get; set; }
        public RefundStatus RefundStatus { get; set; }
        public decimal RefundedAmount { get; set; } = 0;
        public decimal BalanceAmount { get; set; } = 0;
        public string? Reason { get; set; }

        public RefundListItemVM()
        {
            // default constructor
        }

        public RefundListItemVM(Refund refund)
        {
            RefundId = refund.RefundId;
            RefundNo = refund.RefundNo;
            BillId = refund.BillId;
            BillNo = refund.BillNo;
            RefundDate = refund.RefundDate;
            RefundStatus = refund.RefundStatus;
            RefundedAmount = refund.RefundedAmount;
            BalanceAmount = refund.BalanceAmount;
            Reason = refund.Reason;
        }
    }
}