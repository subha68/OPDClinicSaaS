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
        private readonly RefundDAL _refundDAL;
        private readonly VisitDAL _visitDAL;
        private readonly PatientDAL _patientDAL;
        private readonly AccountingService _accounting;


        public PaymentService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
                                : base(config, httpContextAccessor)
        {
            _paymentDAL = new PaymentDAL(config);
            _billDAL = new BillDAL(config);
            _refundDAL = new RefundDAL(config);
            _visitDAL = new VisitDAL(config);
            _patientDAL = new PatientDAL(config);
            _accounting = new AccountingService(config, httpContextAccessor);

            _dal = _paymentDAL;
        }


        // ============================================================
        // CREATE PAYMENT
        // Supports:
        //     Receipt -> Bill
        //     Refund  -> Refund
        // ============================================================
        public override int Create(Payment model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.Amount <= 0)
                throw new InvalidOperationException("Payment amount must be greater than zero.");

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                model.ClinicId = ClinicId;

                int paymentId;

                switch (model.PaymentType)
                {
                    case PaymentType.Receipt:
                        paymentId = CreateReceipt(model, conn, tran);
                        break;

                    case PaymentType.Refund:
                        paymentId = CreateRefundPayment(model, conn, tran);
                        break;

                    default:
                        throw new InvalidOperationException("Payment type has to be either Receipt or Refund.");
                }


                // ----------------------------------------------------
                // Accounting entry for the actual payment
                // ----------------------------------------------------

                model.PaymentId = paymentId;

                Journal journal = _accounting.BuildJournal(model, conn, tran);
                _accounting.PostJournal(journal, conn, tran);

                tran.Commit();

                return paymentId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // ============================================================
        // RECEIPT
        // Patient -> Clinic
        // ============================================================
        private int CreateReceipt(Payment model, SqlConnection conn, SqlTransaction tran)
        {
            if (!model.BillId.HasValue || model.BillId.Value <= 0)
            {
                throw new InvalidOperationException("Bill is required for a receipt.");
            }

            // --------------------------------------------------------
            // Load Bill
            // --------------------------------------------------------

            Bill bill = _billDAL.Get(model.BillId.Value, ClinicId, conn, tran);

            if (bill == null)
            {
                throw new InvalidOperationException("Bill not found.");
            }

            // --------------------------------------------------------
            // Patient must come from Bill
            // --------------------------------------------------------

            model.PatientId = bill.PatientId;
            model.Patient = _patientDAL.Get(model.PatientId, ClinicId, conn, tran);

            if (model.Patient == null)
            {
                throw new InvalidOperationException("Patient associated with the bill was not found.");
            }

            // --------------------------------------------------------
            // Check available bill balance
            // --------------------------------------------------------

            decimal totalPaid = _paymentDAL.GetTotalPaid(bill.BillId, conn, tran);
            decimal balance = bill.NetAmount - totalPaid;

            if (balance <= 0)
            {
                throw new InvalidOperationException("This bill has already been fully paid.");
            }

            if (model.Amount > balance)
            {
                throw new InvalidOperationException($"Payment exceeds the outstanding bill balance of {balance:N2}.");
            }

            model.PaymentType = PaymentType.Receipt;
            model.BillId = bill.BillId;
            model.RefundId = null;

            // --------------------------------------------------------
            // Add payment
            // --------------------------------------------------------

            int paymentId = Add(model, conn, tran);

            // --------------------------------------------------------
            // Update Bill.PaidAmount
            // --------------------------------------------------------

            UpdateBillStatus(bill.BillId, conn, tran);

            return paymentId;
        }


        // ============================================================
        // REFUND PAYMENT
        // Clinic -> Patient
        // ============================================================
        private int CreateRefundPayment(Payment model, SqlConnection conn, SqlTransaction tran)
        {
            if (!model.RefundId.HasValue || model.RefundId.Value <= 0)
            {
                throw new InvalidOperationException("Refund is required for a refund payment.");
            }

            // --------------------------------------------------------
            // Load Refund
            // --------------------------------------------------------

            Refund refund = _refundDAL.Get(model.RefundId.Value, ClinicId, conn, tran);

            if (refund == null)
            {
                throw new InvalidOperationException("Refund not found.");
            }

            // --------------------------------------------------------
            // Refund must already be approved
            // --------------------------------------------------------

            if (refund.RefundStatus != RefundStatus.Approved &&
                refund.RefundStatus != RefundStatus.PartiallyRefunded)
            {
                throw new InvalidOperationException("Only an approved refund can be paid.");
            }

            // --------------------------------------------------------
            // Patient must come from Refund
            // --------------------------------------------------------

            if (!refund.PatientId.HasValue || refund.PatientId.Value <= 0)
            {
                throw new InvalidOperationException("Refund does not have a valid patient.");
            }

            model.PatientId = refund.PatientId.Value;
            model.Patient = _patientDAL.Get(model.PatientId, ClinicId, conn, tran);

            if (model.Patient == null)
            {
                throw new InvalidOperationException("Patient associated with the refund was not found.");
            }

            // --------------------------------------------------------
            // Determine actual amount already refunded
            // --------------------------------------------------------

            decimal totalRefunded = _paymentDAL.GetTotalRefunded(refund.RefundId, conn, tran);
            decimal balance = refund.NetAmount - totalRefunded;

            if (balance <= 0)
            {
                throw new InvalidOperationException("This refund has already been fully paid.");
            }

            if (model.Amount > balance)
            {
                throw new InvalidOperationException($"Payment exceeds the remaining refund balance of {balance:N2}.");
            }

            model.PaymentType = PaymentType.Refund;
            model.RefundId = refund.RefundId;
            model.BillId = null;

            // --------------------------------------------------------
            // Add payment
            // --------------------------------------------------------

            int paymentId = Add(model, conn, tran);

            // --------------------------------------------------------
            // Update Refund.RefundedAmount / Status
            // --------------------------------------------------------

            UpdateRefundStatus(refund.RefundId, conn, tran);

            return paymentId;
        }


        // ============================================================
        // UPDATE BILL PAYMENT STATUS
        // ============================================================
        public void UpdateBillStatus(int billId, SqlConnection conn, SqlTransaction tran)
        {
            Bill bill = _billDAL.Get(billId, ClinicId, conn, tran);

            if (bill == null)
            {
                throw new InvalidOperationException("Bill not found.");
            }

            decimal totalPaid = _paymentDAL.GetTotalPaid(billId, conn, tran);

            if (totalPaid > bill.NetAmount)
            {
                throw new InvalidOperationException("Total payment cannot exceed bill amount.");
            }

            _billDAL.UpdatePayment(billId, totalPaid, conn, tran);
        }

        // ============================================================
        // UPDATE REFUND STATUS
        // ============================================================
        private void UpdateRefundStatus(int refundId, SqlConnection conn, SqlTransaction tran)
        {
            Refund refund = _refundDAL.Get(refundId, ClinicId, conn, tran);

            if (refund == null)
            {
                throw new InvalidOperationException("Refund not found.");
            }

            decimal totalRefunded = _paymentDAL.GetTotalRefunded(refundId, conn, tran);

            if (totalRefunded > refund.NetAmount)
            {
                throw new InvalidOperationException("Total refund payment cannot exceed refund amount.");
            }

            RefundStatus status;

            if (totalRefunded <= 0)
            {
                status = RefundStatus.Approved;
            }
            else if (totalRefunded < refund.NetAmount)
            {
                status = RefundStatus.PartiallyRefunded;
            }
            else
            {
                status = RefundStatus.Completed;
            }

            _refundDAL.UpdateRefund(refundId, totalRefunded, status, UserId, conn, tran);
        }


        // ============================================================
        // GET PAYMENTS BY BILL
        // ============================================================
        public List<Payment> GetPaymentsByBill(int billId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _paymentDAL.GetPaymentsByBill(billId, conn);
        }


        // ============================================================
        // GET TOTAL PAID AGAINST BILL
        // ============================================================
        public decimal GetTotalPaid(int billId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _paymentDAL.GetTotalPaid(billId, conn);
        }


        // ============================================================
        // GET PAYMENTS BY REFUND
        // ============================================================
        public List<Payment> GetPaymentsByRefund(int refundId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _paymentDAL.GetPaymentsByRefund(refundId, conn);
        }


        // ============================================================
        // GET TOTAL REFUNDED
        // ============================================================
        public decimal GetTotalRefunded(int refundId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _paymentDAL.GetTotalRefunded(refundId, conn);
        }


        // ============================================================
        // LIST ALL PAYMENTS FOR PATIENT
        // ============================================================
        public List<Payment> ListByPatient(int patientId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _paymentDAL.ListByPatient(patientId, conn);
        }
    }
}