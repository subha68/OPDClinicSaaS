using DAL;
using Models;

namespace BLL
{
    public class MenuService : BaseService<MenuItem>
    {
        private readonly MenuDAL _menuDAL;

        public MenuService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _menuDAL = new MenuDAL(config);
            _dal = _menuDAL;
        }

        public List<MenuItem> GetMenu(string featureCsv)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _menuDAL.GetMenuItems(featureCsv, conn);
            }
            catch
            {
                throw;
            }
        }
    }
}

