using DAL;
using Models;

namespace BLL
{
    public class BillingChargeService : BaseService<BillingCharge>
    {
        private readonly BillingChargeDAL _billingChargeDAL;

        public BillingChargeService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(httpContextAccessor)
        {
            _billingChargeDAL = new BillingChargeDAL(config);
            _dal = _billingChargeDAL;
        }

        public override List<BillingCharge> List()
        {
            return _billingChargeDAL.GetAllByClinic(ClinicId);
        }

    }
}
