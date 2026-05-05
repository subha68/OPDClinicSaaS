using DAL;
using Models;

public class RoleService
{
    private readonly RoleDAL _dal;

    public RoleService(RoleDAL dal)
    {
        _dal = dal;
    }

    public List<Role> GetRoles() => _dal.GetRoles();

    public void CreateRole(Role role)
    {
        _dal.Add(role);
    }
}