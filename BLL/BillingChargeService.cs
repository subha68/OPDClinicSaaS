using DAL;
using Models;

namespace BLL
{
    public class BillingChargeService : BaseService<BillingCharge>
    {
        private readonly BillingChargeDAL _billingChargeDAL;

        public BillingChargeService(IConfiguration config)
        {
            _billingChargeDAL = new BillingChargeDAL(config);
            _dal = _billingChargeDAL;
        }

        public List<BillingCharge> ListByClinic(int clinicId)
        {
            return _billingChargeDAL.GetAllByClinic(clinicId);
        }

    }
}
