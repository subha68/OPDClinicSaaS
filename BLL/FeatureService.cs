using DAL;
using Models;

namespace BLL
{
    public class FeatureService
    {
        private readonly FeatureDAL _dal;

        public FeatureService(IConfiguration config)
        {
            _dal = new FeatureDAL(config);
        }

        public List<Feature> GetFeatures(int roleId)
        {
            return _dal.GetFeaturesByRole(roleId);
        }
    }
}
