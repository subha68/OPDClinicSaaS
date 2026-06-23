using DAL;
using Models;
using Microsoft.Data.SqlClient;

namespace BLL
{
    public class BillingChargeService : BaseService<BillingCharge>
    {
        private readonly BillingChargeDAL _billingChargeDAL;

        public BillingChargeService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _billingChargeDAL = new BillingChargeDAL(config);
            _dal = _billingChargeDAL;
        }

        public override List<BillingCharge> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var charges = _billingChargeDAL.GetAllByClinic(ClinicId, conn);
                return charges;
            }
            catch
            {
                throw;
            }

        }

        public List<BillingCharge> Search(string term)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var charges = _billingChargeDAL.Search(ClinicId, term, conn);
                return charges;
            }
            catch
            {
                throw;
            }
            
        }

    }
}
