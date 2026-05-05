using DAL;
using Models;

public class UserService
{
    private readonly UserDAL _dal;

    public UserService(UserDAL dal)
    {
        _dal = dal;
    }

    public void CreateUser(User user)
    {
        // 🔐 Hash password (basic for now)
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        _dal.Add(user);
    }

    public List<User> GetUsers() => _dal.GetUsers();

}