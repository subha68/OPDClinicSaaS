using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DAL;
using Helpers;
using Models;

namespace BLL
{
    public class UserService : BaseService<User>
    {
        private readonly UserDAL _userDAL;
        private readonly RoleDAL _roleDAL;
        private readonly DoctorDAL _doctorDAL;
        private readonly FeatureDAL _featureDAL;

        public UserService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                            : base(config, httpContextAccessor)
        {
            _userDAL = new UserDAL(config);
            _roleDAL = new RoleDAL(config);
            _doctorDAL = new DoctorDAL(config);
            _featureDAL = new FeatureDAL(config);
            _dal = _userDAL;
        }

        public override int Add(User user)
        {
            user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);

            return base.Add(user);
        }

        public override bool Update(User user)
        {
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash = Get(user.UserId).PasswordHash;
            }
            else
            {
                user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);
            }
            
            return base.Update(user);
        }

        public User ValidateUser(string username, string password)
        {
            bool success = false;
            var user = _userDAL.GetUserByName(username, ClinicId);
            if (user != null)
            {
                success = PasswordHelper.VerifyPassword(user.PasswordHash, password);
            }   

            if (success)
                return user;
            else
                return null;
        }

        public Role GetUserRole(User user)
        {
            return _roleDAL.Get(user.RoleId);
        }

        public Doctor GetUserDoctor(User user)
        {
            int doctorId = _doctorDAL.GetDoctorIdByUserId(user.UserId);
            if (doctorId == 0) 
                return null;
            else
                return _doctorDAL.Get(doctorId);
        }

        public string GetUserFeaturesAsString(User user)
        {
            var features = _featureDAL.GetFeaturesByRole(user.RoleId);
            return string.Join(",", features.Select(f => f.Code));
        }

        public bool SetupSuperAdmin()
        {
            var superAdmin = new User
            {
                Username = "superadmin",
                PasswordHash = "admin123",
                RoleId = 1,
                FirstName = "Super",
                LastName = "Admin",
                IsActive = true,
                IsFirstLogin = false
            };

            int userId = _userDAL.GetUserIdByName(superAdmin.Username);

            if (userId == 0)
            {
                userId = Add(superAdmin);
            }
            else
            {
                superAdmin.UserId = userId;
                Update(superAdmin);
            }

            return true;
        }


        public List<User> ListByClinic() => _userDAL.ListByClinic(ClinicId);
        public bool UsernameExists(string username) => _userDAL.GetUserCountByName(username) > 0;

    }
}