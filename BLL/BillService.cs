using DAL;
using Models;
using ViewModels;

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
            _dal = _billDAL;
        }

        public override int Add(Bill bill)
        {
            bill.Visit = _visitDAL.Get(bill.VisitId);
            bill.PatientId = bill.Visit.PatientId;
            bill.Patient = _patientDAL.Get(bill.PatientId);
            bill.ClinicId = ClinicId;
            bill.Clinic = _clinicDAL.Get(bill.ClinicId);

            bill.GrossAmount = bill.BillLines.Sum(x => x.Rate * x.Quantity);
            bill.DiscountAmount = bill.BillLines.Sum(x => (x.Rate * x.Quantity * x.DiscountPercent / 100));

            int billId = base.Add(bill);

            foreach (var line in bill.BillLines)
            {
                line.BillId = billId;
                line.BillingCharge = _billingChargeDAL.Get(line.ChargeId);
                line.ChargeCode = line.BillingCharge.ChargeCode;
                line.ChargeDescription = line.BillingCharge.Description;
                _billLineDAL.Add(line);
            }

            return billId;
        }

        public override Bill Get(int id)
        {
            var bill = base.Get(id);
            if (bill != null)
            {
                bill.Clinic = _clinicDAL.Get(ClinicId);
                bill.ClinicId = ClinicId;
                bill.Patient = _patientDAL.Get(bill.PatientId);
                bill.PatientId = bill.Patient.PatientId;
                bill.Visit = _visitDAL.Get(bill.VisitId);
                bill.VisitId = bill.Visit.VisitId;
                bill.VisitDate = bill.Visit?.VisitDate;
                bill.BillLines = _billLineDAL.GetByBillId(id);

                foreach (var line in bill.BillLines)
                {
                    line.BillingCharge = _billingChargeDAL.Get(line.ChargeId);
                    line.ChargeCode = line.BillingCharge.ChargeCode;
                    line.ChargeDescription = line.BillingCharge.Description;
                }

                bill.Payments = _paymentDAL.GetPaymentsByBill(id);
                bill.TotalPaidAmount = _paymentDAL.GetTotalPaid(id);
            }
            return bill;
        }

        public override List<Bill> List()
        {
            return _billDAL.GetAllByClinic(ClinicId);
        }

        public List<Bill> ListByPatient(int patientId)
        {
            return _billDAL.ListByPatient(ClinicId, patientId);
        }

    }
}
