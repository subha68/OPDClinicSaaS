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

        public BillService(IConfiguration config)
        {
            _billDAL = new BillDAL(config);
            _billLineDAL = new BillLineDAL(config);
            _billingChargeDAL = new BillingChargeDAL(config);
            _dal = _billDAL;
        }

        public override int Add(Bill bill)
        {
            int billId = base.Add(bill);

            foreach (var line in bill.BillLines)
            {
                line.BillId = billId;
                _billLineDAL.Add(line);
            }

            return billId;
        }

        public override Bill Get(int id)
        {
            var bill = base.Get(id);
            if (bill != null)
            {
                bill.BillLines = _billLineDAL.GetByBillId(id);
                foreach (var line in bill.BillLines)
                {
                    line.BillingCharge = _billingChargeDAL.Get(line.BillingChargeId);
                }
            }
            return bill;
        }

        public List<Bill> ListByClinic(int clinicId)
        {
            return _billDAL.GetAllByClinic(clinicId);
        }

        public List<BillListVM> ListByPatient(int clinicId, int patientId)
        {
            return _billDAL.GetBillsByPatient(clinicId, patientId);
        }

    }
}
