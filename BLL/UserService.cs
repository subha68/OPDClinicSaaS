using DAL;
using Models;

public class UserService
{
    private readonly UserDAL _dal;

    public UserService(IConfiguration config)
    {
        _dal = new UserDAL(config);
    }

    public int CreateUser(User user)
    {
        // 🔐 Hash password (basic for now)
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        return _dal.Add(user);
    }

    public List<User> GetUsers() => _dal.GetUsers();

    public bool UsernameExists(string username) => _dal.GetUserCountByName(username) > 0;

}