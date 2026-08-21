using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DAL;
using Helpers;
using Models;
using ViewModels;
using Microsoft.Data.SqlClient;

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

        public override int Add(User user, SqlConnection conn, SqlTransaction tran)
        {
            user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);

            return base.Add(user, conn, tran);

        }

        public override bool Update(User user, SqlConnection conn, SqlTransaction tran)
        {
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash = Get(user.UserId).PasswordHash;
            }
            else
            {
                user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);
            }
            
            return base.Update(user, conn, tran);
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
                return _roleDAL.Get(user.RoleId, ClinicId, conn);
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
                    return _doctorDAL.Get(doctorId, ClinicId, conn);
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

        public ChangePasswordResult ChangePassword(int userId, string currentPassword, string newPassword)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                var user = _userDAL.Get(userId, ClinicId, conn, tran);

                if (user == null)
                {
                    return new ChangePasswordResult
                    {
                        Success = false,
                        Message = "User account could not be found."
                    };
                }

                if (!PasswordHelper.VerifyPassword(user.PasswordHash, currentPassword))
                {
                    return new ChangePasswordResult
                    {
                        Success = false,
                        Message = "The current password is incorrect."
                    };
                }

                if (PasswordHelper.VerifyPassword(user.PasswordHash, newPassword))
                {
                    return new ChangePasswordResult
                    {
                        Success = false,
                        Message = "The new password must be different from the current password."
                    };
                }

                user.PasswordHash = newPassword;
                user.IsFirstLogin = false;

                bool result = Update(user, conn, tran);

                if (!result)
                {
                    tran.Rollback();

                    return new ChangePasswordResult
                    {
                        Success = false,
                        Message = "Unable to change the password."
                    };
                }

                tran.Commit();

                return new ChangePasswordResult
                {
                    Success = true,
                    Message = "Your password has been changed successfully."
                };
            }
            catch
            {
                tran.Rollback();
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