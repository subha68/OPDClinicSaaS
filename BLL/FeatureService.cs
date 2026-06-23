using DAL;
using Models;

namespace BLL
{
    public class FeatureService : BaseService<Feature>
    {
        private readonly FeatureDAL _featureDAL;

        public FeatureService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _featureDAL = new FeatureDAL(config);
            _dal = _featureDAL;
        }

        public List<Feature> GetFeatures(int roleId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _featureDAL.GetFeaturesByRole(roleId, conn);
            }
            catch
            {
                throw;
            }

        }
    }
}
