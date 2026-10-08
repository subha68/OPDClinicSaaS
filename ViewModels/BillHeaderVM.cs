using Models;

namespace ViewModels
{
    public class BillHeaderVM
    {
        public int BillId { get; set; }
        public string BillNo { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime VisitDate { get; set; }
        public decimal BillAmount { get; set; }
        public decimal RefundedAmount { get; set; }
        public decimal BalanceAmount { get; set; }

        public BillHeaderVM()
        {
            // default constructor
        }

        public BillHeaderVM(Bill bill)
        {
            BillId = bill.BillId;
            BillNo = bill.BillNo;
            BillDate = bill.BillDate;
            VisitDate = bill.VisitDate!.Value;
            BillAmount = bill.NetAmount;
        }
    }
}