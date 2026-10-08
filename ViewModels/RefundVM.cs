using Models;

namespace ViewModels
{
    public class RefundVM
    {
        public int? PatientId { get; set; }
        public List<Refund> Refunds { get; set; } = [];
        public List<Bill> Bills { get; set; } = [];
        public List<BillLine> BillLines { get; set; } = [];
        public decimal TotalGrossAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalRefundAmount { get; set; }
    }
}