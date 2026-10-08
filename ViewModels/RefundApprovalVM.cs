using Models;

namespace ViewModels
{
    public class RefundApprovalVM
    {
        public List<Refund> PendingRefunds { get; set; } = new();

        public Refund? SelectedRefund =>
            PendingRefunds.Count == 1
                ? PendingRefunds[0]
                : null;
    }
}