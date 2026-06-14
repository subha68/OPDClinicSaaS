using DAL;
using Models;
using ViewModels;
using Microsoft.Data.SqlClient;

namespace BLL
{
    public class PaymentService : BaseService<Payment>
    {
        private readonly PaymentDAL _paymentDAL;
        private readonly BillDAL _billDAL;
        private readonly VisitDAL _visitDAL;

        public PaymentService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _paymentDAL = new PaymentDAL(config);
            _billDAL = new BillDAL(config);
            _visitDAL = new VisitDAL(config);
            _dal = _paymentDAL;
        }

        public override int Create(Payment model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                int id = Add(model, conn, tran);
                UpdateBillStatus(model.BillId, conn, tran);
                tran.Commit();
                return id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public void UpdateBillStatus(int billId, SqlConnection conn, SqlTransaction tran)
        {
            Bill bill = _billDAL.Get(billId);

            decimal totalPaid = _paymentDAL.GetTotalPaid(billId, conn, tran);

            _billDAL.UpdatePayment(billId, totalPaid, conn, tran);
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