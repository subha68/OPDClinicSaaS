using DAL;
using Models;

public class RoleService
{
    private readonly RoleDAL _dal;

    public RoleService(IConfiguration config)
    {
        _dal = new RoleDAL(config);
    }


    public List<Role> GetRoles() => _dal.GetRoles();

    public int GetRoleIdByName(string roleName)
    {
        var role = _dal.GetRoleByName(roleName);
        if (role == null)
            throw new Exception($"Role '{roleName}' not found.");

        return role.RoleId;
    }

    public string GetRoleName(int roleId)
    {
        var role = _dal.GetRoleById(roleId);
        if (role == null)
            throw new Exception($"Role with ID '{roleId}' not found.");

        return role.RoleName;
    }
    
    public void CreateRole(Role role)
    {
        _dal.Add(role);
    }
}