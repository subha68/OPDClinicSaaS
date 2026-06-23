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

        public override int Create(User user)
        {
            user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                int id = Add(user, conn, tran);
                tran.Commit();
                return id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public override bool Modify(User user)
        {
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash = Get(user.UserId).PasswordHash;
            }
            else
            {
                user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);
            }
            
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                bool result = Update(user, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public User ValidateUser(string username, string password)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                bool success = false;
                var user = _userDAL.GetUserByName(username, ClinicId, conn);
                if (user != null)
                {
                    success = PasswordHelper.VerifyPassword(user.PasswordHash, password);
                }   

                if (success)
                    return user;
                else
                    return null;
            }
            catch
            {
                throw;
            }
        }

        public Role GetUserRole(User user)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _roleDAL.Get(user.RoleId, conn);
            }
            catch
            {
                throw;
            }
        }

        public Doctor GetUserDoctor(User user)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                int doctorId = _doctorDAL.GetDoctorIdByUserId(user.UserId, conn);
                if (doctorId == 0) 
                    return null;
                else
                    return _doctorDAL.Get(doctorId, conn);
            }
            catch
            {
                throw;
            }
        }

        public string GetUserFeaturesAsString(User user)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var features = _featureDAL.GetFeaturesByRole(user.RoleId, conn);
                return string.Join(",", features.Select(f => f.Code));
            }
            catch
            {
                throw;
            }
        }

        public bool SetupSuperAdmin()
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
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

                int userId = _userDAL.GetUserIdByName(superAdmin.Username, conn);

                if (userId == 0)
                {
                    userId = Add(superAdmin, conn, tran);
                }
                else
                {
                    superAdmin.UserId = userId;
                    Update(superAdmin, conn, tran);
                }

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public List<User> ListByClinic()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _userDAL.ListByClinic(ClinicId, conn);
            }
            catch
            {
                throw;
            }
        }

        public bool UsernameExists(string username)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _userDAL.GetUserCountByName(username, conn) > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}