using DAL;
using Models;

namespace BLL
{
    public class UserService : BaseService<User>
    {
        private readonly UserDAL _userDAL;

        public UserService(IConfiguration config)
        {
            _userDAL = new UserDAL(config);
            _dal = _userDAL;
        }

        public override int Add(User user)
        {
            // 🔐 Hash password (basic for now)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            return base.Add(user);
        }

        public override bool Update(User user)
        {
            // 🔐 Hash password (basic for now)
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash = Get(user.UserId).PasswordHash;
            }
            else
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }
            
            return base.Update(user);
        }

        public List<User> ListByClinic(int clinicId) => _userDAL.GetAllByClinic(clinicId);
        public bool UsernameExists(string username) => _userDAL.GetUserCountByName(username) > 0;

    }
}