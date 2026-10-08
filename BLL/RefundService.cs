using DAL;
using DocumentFormat.OpenXml.Office2010.Drawing.Charts;
using Helpers;
using Microsoft.Data.SqlClient;
using Models;
using ViewModels;

namespace BLL
{
    public class RefundService : BaseService<Refund>
    {
        private readonly RefundDAL _refundDAL;
        private readonly BillDAL _billDAL;
        private readonly BillLineDAL _billLineDAL;
        private readonly RefundLineDAL _refundLineDAL;
        private readonly PaymentDAL _paymentDAL;
        private readonly AccountingService _accounting;
        private readonly ClinicDAL _clinicDAL;

        public RefundService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
                            : base(config, httpContextAccessor)
        {
            _refundDAL = new RefundDAL(config);
            _billDAL = new BillDAL(config);
            _billLineDAL = new BillLineDAL(config);
            _refundLineDAL = new RefundLineDAL(config);
            _paymentDAL = new PaymentDAL(config);
            _accounting = new AccountingService(config, httpContextAccessor);
            _clinicDAL = new ClinicDAL(config);

            _dal = _refundDAL;
        }

        private void ValidateRefund(Refund refund)
        {
            if (refund == null)
                throw new ArgumentNullException(nameof(refund));

            if (refund.BillId <= 0)
                throw new InvalidOperationException("Bill is required.");

            if (refund.RefundDate == DateTime.MinValue)
                throw new InvalidOperationException("Refund date is required.");

            if (refund.Lines == null || !refund.Lines.Any())
                throw new InvalidOperationException("Refund must contain at least one line.");
        }

        private void CalculateTotals(Refund refund)
        {
            refund.GrossAmount = refund.Lines.Sum(x => x.GrossAmount);
            refund.DiscountAmount = refund.Lines.Sum(x => x.DiscountAmount);
            refund.TaxAmount = refund.Lines.Sum(x => x.TaxAmount);

            refund.CGST = refund.TaxAmount / 2;
            refund.SGST = refund.TaxAmount / 2;
            refund.IGST = 0;
        }

        private Refund LoadRefund(int refundId, SqlConnection conn, SqlTransaction? tran = null)
        {
            Refund refund = _refundDAL.Get(refundId, ClinicId, conn, tran);

            if (refund == null)
                throw new InvalidOperationException("Refund not found.");

            refund.Lines = _refundLineDAL.GetByRefundId(refundId, conn, tran);

            refund.Bill = _billDAL.Get(refund.BillId, ClinicId, conn, tran);

            refund.Payments = _paymentDAL.GetPaymentsByRefund(refundId, conn, tran);

            refund.Clinic = _clinicDAL.Get(ClinicId, ClinicId, conn, tran);

            return refund;
        }

        public override int Create(Refund refund)
        {
            ValidateRefund(refund);

            CalculateTotals(refund);

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                refund.ClinicId = ClinicId;
                //refund.RefundStatus = RefundStatus.Draft;

                if (refund.RefundStatus == RefundStatus.PendingApproval)
                {
                    refund.RequestedBy = UserId;
                    refund.RequestedOn = DateTime.Today;
                }

                refund.RefundNo = _refundDAL.GetNextNo(ClinicId, refund.RefundDate, "Refund", conn, tran);

                int refundId = _refundDAL.Add(refund, conn, tran);

                refund.RefundId = refundId;

                int lineNo = 1;

                foreach (var line in refund.Lines)
                {
                    line.RefundId = refundId;
                    line.RefundLineNo = lineNo++;
                    
                }

                _refundLineDAL.AddLines(refund.Lines, conn, tran);

                List<BillLine> billLines = new List<BillLine>();

                // reduce available quantities for new refund lines
                ReduceAvailableQuantities(refund.BillId, refund.Lines, conn, tran);

                tran.Commit();

                return refundId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public override bool Modify(Refund refund)
        {
            ValidateRefund(refund);

            CalculateTotals(refund);

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                Refund existing = LoadRefund(refund.RefundId, conn, tran);

                if (existing.RefundStatus != RefundStatus.Draft)
                    throw new InvalidOperationException("Only draft refunds can be edited.");

                _refundDAL.Update(refund, conn, tran);

                if (existing.RefundStatus == RefundStatus.Draft &&
                    refund.RefundStatus == RefundStatus.PendingApproval)
                    _refundDAL.SubmitForApproval(refund.RefundId, UserId, conn, tran);

                // restore available quantities for existing refund lines
                RestoreAvailableQuantities(existing.BillId, existing.Lines, conn, tran);

                _refundLineDAL.DeleteLines(refund.RefundId, ClinicId, conn, tran);

                int lineNo = 1;

                foreach (var line in refund.Lines)
                {
                    line.RefundId = refund.RefundId;
                    line.RefundLineNo = lineNo++;
                }

                _refundLineDAL.AddLines(refund.Lines, conn, tran);

                // reduce available quantities for new refund lines
                ReduceAvailableQuantities(refund.BillId, refund.Lines, conn, tran);

                tran.Commit();

                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool RestoreAvailableQuantities(int billId, List<RefundLine> existingRefundLines, 
                                                SqlConnection conn, SqlTransaction tran)
        {       
            List<BillLine> billLines = new List<BillLine>();

            foreach (var existingRefundLine in existingRefundLines)
            {
                var billLine = new BillLine();
                billLine.BillId = billId;
                billLine.LineId = existingRefundLine.BillLineId;
                billLine.AvailableQuantity = -1 * existingRefundLine.Quantity;  // this quantity needs to be increased

                billLines.Add(billLine);
            }

            _billLineDAL.UpdateAvailableQuantities(billLines, conn, tran);

            return true;
        }

        public bool ReduceAvailableQuantities(int billId, List<RefundLine> newRefundLines, 
                                                SqlConnection conn, SqlTransaction tran)
        {
            List<BillLine> billLines = new List<BillLine>();

            foreach (var refundLine in newRefundLines)
            {
                var billLine = new BillLine();
                billLine.BillId = billId;
                billLine.LineId = refundLine.BillLineId;
                billLine.AvailableQuantity = refundLine.Quantity;  // this quantity needs to be reduced

                billLines.Add(billLine);
            }

            _billLineDAL.UpdateAvailableQuantities(billLines, conn, tran);

            return true;
        }

        public Refund GetRefund(int refundId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return LoadRefund(refundId, conn);
        }

        public List<Refund> ListRefunds()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _refundDAL.GetAll(ClinicId, conn);
        }

        public List<Refund> ListByStatus(RefundStatus status)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _refundDAL.ListByStatus(ClinicId, status, conn);
        }

        public List<Refund> ListApprovedNotCompleted()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _refundDAL.ListApprovedNotCompleted(ClinicId, conn);
        }

        public List<Refund> ListPendingApproval()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _refundDAL.ListByStatus(ClinicId, RefundStatus.PendingApproval, conn);
        }

        public List<Refund> ListByPatient(int patientId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _refundDAL.ListByPatient(ClinicId, patientId, conn);
        }

        public List<Refund> ListByBill(int billId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _refundDAL.ListByBill(ClinicId, billId, conn);
        }

        public void SubmitForApproval(int refundId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                Refund refund = LoadRefund(refundId, conn, tran);

                if (refund.RefundStatus != RefundStatus.Draft)
                    throw new InvalidOperationException("Only draft refunds can be submitted.");

                _refundDAL.SubmitForApproval(refundId, UserId, conn, tran);

                refund.RefundStatus = RefundStatus.PendingApproval;

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public void Approve(int refundId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                Refund refund = LoadRefund(refundId, conn, tran);

                if (refund.RefundStatus != RefundStatus.PendingApproval)
                    throw new InvalidOperationException("Refund is not awaiting approval.");

                CalculateTotals(refund);

                _refundDAL.Approve(refundId, UserId, conn, tran);

                refund.RefundStatus = RefundStatus.Approved;

                Journal journal = _accounting.BuildJournal(refund, conn, tran);

                _accounting.PostJournal(journal, conn, tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public void Reject(int refundId, string reason)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                Refund refund = LoadRefund(refundId, conn, tran);

                if (refund.RefundStatus != RefundStatus.PendingApproval)
                    throw new InvalidOperationException("Refund is not awaiting approval.");

                _refundDAL.Reject(refundId, UserId, reason, conn, tran);

                refund.RefundStatus = RefundStatus.Rejected;

                // increase available quantity for bill line
                RestoreAvailableQuantities(refund.BillId, refund.Lines, conn, tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public void Cancel(int refundId, string reason)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                Refund refund = LoadRefund(refundId, conn, tran);

                if (refund.RefundStatus != RefundStatus.Draft)
                    throw new InvalidOperationException("Only draft refund can be cancelled.");

                _refundDAL.Cancel(refundId, UserId, reason, conn, tran);

                refund.RefundStatus = RefundStatus.Cancelled;

                // increase available quantity for bill line
                RestoreAvailableQuantities(refund.BillId, refund.Lines, conn, tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public BillHeaderVM PrepareBillHeader(int billId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var bill = _billDAL.Get(billId, ClinicId, conn);

                if (bill == null)
                    return new BillHeaderVM();

                bill.BillLines = _billLineDAL.GetByBillId(billId, conn);

                var refundedAmount = _refundDAL
                                        .ListByBill(ClinicId, billId, conn)
                                        .Where(r => r.RefundStatus != RefundStatus.Cancelled)
                                        .Sum(r => r.NetAmount);

                var balanceAmount = bill.NetAmount - refundedAmount;

                var vm = new BillHeaderVM(bill);
                vm.RefundedAmount = refundedAmount;
                vm.BalanceAmount = balanceAmount;

                return vm;
            }
            catch
            {
                throw;
            }

        }

        public RefundLine PrepareRefundLine(int billId, int billLineId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var bill = _billDAL.Get(billId, ClinicId, conn);

                if (bill == null)
                    return new RefundLine();

                bill.BillLines = _billLineDAL.GetByBillId(billId, conn);

                var billLine = bill.BillLines
                        .FirstOrDefault(x => x.LineId == billLineId);

                if (billLine == null)
                    return new RefundLine();

                if (billLine.AvailableQuantity <= 0)
                    return new RefundLine();

                var refundLine = new RefundLine(billLine);

                return refundLine;
            }
            catch
            {
                throw;
            }
        }

        public List<BillLine> ListRefundableBillLines(int billId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _billLineDAL.ListRefundableBillLines(ClinicId, billId, conn);
            }
            catch
            {
                throw;
            }
            
        }

        public Refund GetPendingApproval(int refundId)
        {
            Refund refund = GetRefund(refundId);

            if (refund == null)
                throw new InvalidOperationException("Refund not found.");

            if (refund.RefundStatus != RefundStatus.PendingApproval)
                throw new InvalidOperationException(
                    "Refund is not awaiting approval.");

            return refund;
        }

        // Complete status will be set from Payment page

    }
}
