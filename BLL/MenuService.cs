using DAL;
using Models;

namespace BLL
{
    public class MenuService
    {
        private readonly MenuDAL _dal;

        public MenuService(IConfiguration config)
        {
            _dal = new MenuDAL(config);
        }

        public List<MenuItem> GetMenu(string featureCsv)
        {
            return _dal.GetMenuItems(featureCsv);
        }
    }
}

