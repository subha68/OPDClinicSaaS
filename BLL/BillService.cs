using DAL;
using Models;
using ViewModels;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BLL
{
    public class BillService : BaseService<Bill>
    {
        private readonly BillDAL _billDAL;
        private readonly BillLineDAL _billLineDAL;
        private readonly BillingChargeDAL _billingChargeDAL;
        private readonly PatientDAL _patientDAL;
        private readonly PaymentDAL _paymentDAL;
        private readonly VisitDAL _visitDAL;
        private readonly ClinicDAL _clinicDAL;
        private readonly AccountingService _accounting;

        public BillService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _billDAL = new BillDAL(config);
            _billLineDAL = new BillLineDAL(config);
            _billingChargeDAL = new BillingChargeDAL(config);
            _patientDAL = new PatientDAL(config);
            _paymentDAL = new PaymentDAL(config);
            _visitDAL = new VisitDAL(config);
            _clinicDAL = new ClinicDAL(config);
            _accounting = new AccountingService(config, httpContextAccessor);
            _dal = _billDAL;
        }

        public override int Create(Bill bill)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                bill.Visit = _visitDAL.Get(bill.VisitId, conn, tran);
                bill.PatientId = bill.Visit.PatientId;
                bill.Patient = _patientDAL.Get(bill.PatientId, conn, tran);
                bill.ClinicId = ClinicId;
                bill.Clinic = _clinicDAL.Get(bill.ClinicId, conn, tran);

                bill.GrossAmount = bill.BillLines.Sum(x => x.GrossAmount);
                bill.DiscountAmount = bill.BillLines.Sum(x => x.DiscountAmount);

                int billId = Add(bill, conn, tran);
                bill.BillId = billId;

                foreach (var line in bill.BillLines)
                {
                    line.BillId = billId;
                    line.PatientId = bill.PatientId;

                    if (bill.BillType == "Service")
                    {
                        int chargeId = line.ReferenceId;
                        var charge = _billingChargeDAL.Get(chargeId, conn, tran);
                        line.ReferenceCode = charge.ChargeCode;
                        line.Description = charge.Description;
                        line.Rate = charge.Rate;
                        line.ServiceType = charge.ServiceType;
                    }

                    _billLineDAL.Add(line, conn, tran);
                }

                Journal journal = _accounting.BuildJournal(bill, conn, tran);
                _accounting.PostJournal(journal, conn, tran);

                tran.Commit();
                return billId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public override Bill Get(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var bill = base.Get(id);
                if (bill != null)
                {
                    bill.Clinic = _clinicDAL.Get(ClinicId, conn);
                    bill.ClinicId = ClinicId;
                    bill.Patient = _patientDAL.Get(bill.PatientId, conn);
                    bill.PatientId = bill.Patient.PatientId;
                    bill.Visit = _visitDAL.Get(bill.VisitId, conn);
                    bill.VisitId = bill.Visit.VisitId;
                    bill.VisitDate = bill.Visit?.VisitDate;
                    bill.BillLines = _billLineDAL.GetByBillId(id, conn);

                    /*foreach (var line in bill.BillLines)
                    {
                        line.BillingCharge = _billingChargeDAL.Get(line.ChargeId);
                        line.ChargeCode = line.BillingCharge.ChargeCode;
                        line.ChargeDescription = line.BillingCharge.Description;
                    }*/

                    bill.Payments = _paymentDAL.GetPaymentsByBill(id, conn);
                    bill.TotalPaidAmount = _paymentDAL.GetTotalPaid(id, conn);
                }

                return bill;
            }
            catch
            {
                throw;
            }
            
        }

        public override List<Bill> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _billDAL.GetAllByClinic(ClinicId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<Bill> ListByPatient(int patientId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _billDAL.ListByPatient(ClinicId, patientId, conn);
            }
            catch
            {
                throw;
            }

        }

    }
}
