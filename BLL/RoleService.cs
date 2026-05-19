using DAL;
using Models;

namespace BLL
{
    public class RoleService : BaseService<Role>
    {
        private readonly RoleDAL _roleDAL;

        public RoleService(IConfiguration config)
        {
            _roleDAL = new RoleDAL(config);
            _dal = _roleDAL;
        }


        public List<Role> GetRoles() => _dal.GetAll();

        public int GetRoleIdByName(string roleName)
        {
            var role = _roleDAL.GetRoleByName(roleName);
            if (role == null)
                throw new Exception($"Role '{roleName}' not found.");

            return role.RoleId;
        }

        public string GetRoleName(int roleId)
        {
            var role = _dal.Get(roleId);
            if (role == null)
                throw new Exception($"Role with ID '{roleId}' not found.");

            return role.RoleName;
        }
        
        public void CreateRole(Role role)
        {
            _dal.Add(role);
        }
    }
}