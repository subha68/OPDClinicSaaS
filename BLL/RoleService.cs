using DAL;
using Models;

namespace BLL
{
    public class RoleService : BaseService<Role>
    {
        private readonly RoleDAL _roleDAL;

        public RoleService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _roleDAL = new RoleDAL(config);
            _dal = _roleDAL;
        }

        public Role GetRoleByName(string roleName)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var role = _roleDAL.GetRoleByName(roleName, conn);
                if (role == null)
                    throw new Exception($"Role '{roleName}' not found.");

                return role;
            }
            catch
            {
                throw;
            }
        }

    }
}