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
        private readonly PatientDAL _patientDAL;
        private readonly AccountingService _accounting;


        public PaymentService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _paymentDAL = new PaymentDAL(config);
            _billDAL = new BillDAL(config);
            _visitDAL = new VisitDAL(config);
            _patientDAL = new PatientDAL(config);
            _accounting = new AccountingService(config, httpContextAccessor);
            _dal = _paymentDAL;
        }

        public override int Create(Payment model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                model.ClinicId = ClinicId;
                model.Patient = _patientDAL.Get(model.PatientId, conn, tran);
                int id = Add(model, conn, tran);
                UpdateBillStatus(model.BillId, conn, tran);
                model.PaymentId = id;
                
                Journal journal = _accounting.BuildJournal(model, conn, tran);
                _accounting.PostJournal(journal, conn, tran);

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
            Bill bill = _billDAL.Get(billId, conn, tran);

            decimal totalPaid = _paymentDAL.GetTotalPaid(billId, conn, tran);

            _billDAL.UpdatePayment(billId, totalPaid, conn, tran);
        }

        public List<Payment> GetPaymentsByBill(int billId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _paymentDAL.GetPaymentsByBill(billId, conn);
            }
            catch
            {
                throw;
            }
        }

        public decimal GetTotalPaid(int billId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _paymentDAL.GetTotalPaid(billId, conn);
            }
            catch
            {
                throw;
            }
        }

    }
}