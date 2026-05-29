using DAL;
using Models;

namespace BLL
{
    public class PaymentService : BaseService<Payment>
    {
        private readonly PaymentDAL _paymentDAL;
        private readonly BillDAL _billDAL;

        public PaymentService(IConfiguration config)
        {
            _paymentDAL = new PaymentDAL(config);
            _billDAL = new BillDAL(config);
            _dal = _paymentDAL;
        }

        public override int Add(Payment payment)
        {
            int paymentId = base.Add(payment);

            UpdateBillStatus(payment.BillId);

            return paymentId;
        }

        public void UpdateBillStatus(int billId)
        {
            Bill bill = _billDAL.Get(billId);

            decimal totalPaid = _paymentDAL.GetTotalPaid(billId);

            string status = "Due";

            if (totalPaid > 0 && totalPaid < bill.NetAmount)
            {
                status = "Partially Paid";
            }
            else if (totalPaid >= bill.NetAmount)
            {
                status = "Paid";
            }

            _billDAL.UpdateStatus(billId, status);
        }

        public List<Payment> GetPaymentsByBill(int billId)
        {
            return _paymentDAL.GetPaymentsByBill(billId);
        }

        public decimal GetTotalPaid(int billId)
        {
            return _paymentDAL.GetTotalPaid(billId);
        }
    }
}