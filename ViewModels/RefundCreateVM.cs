using Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ViewModels
{
    public class RefundCreateVM
    {
        public Refund Refund { get; set; }
        public List<SelectListItem> Bills { get; set; } = [];
        public List<BillLine> BillLines { get; set; } = [];
        public decimal TotalGrossAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalRefundAmount { get; set; }
    }
}