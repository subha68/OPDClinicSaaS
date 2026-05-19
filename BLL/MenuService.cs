using DAL;
using Models;

namespace BLL
{
    public class MenuService : BaseService<MenuItem>
    {
        private readonly MenuDAL _menuDAL;

        public MenuService(IConfiguration config)
        {
            _menuDAL = new MenuDAL(config);
            _dal = _menuDAL;
        }

        public List<MenuItem> GetMenu(string featureCsv)
        {
            return _menuDAL.GetMenuItems(featureCsv);
        }
    }
}

