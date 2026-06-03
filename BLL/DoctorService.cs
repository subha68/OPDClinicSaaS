using DAL;
using Models;
using ViewModels;
using Helpers;

namespace BLL
{    public class DoctorService : BaseService<Doctor>
    {
        private readonly DoctorDAL _doctorDAL;
        private readonly RoleDAL _roleDAL;
        private readonly UserService _user;

        public DoctorService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _doctorDAL = new DoctorDAL(config);
            _roleDAL = new RoleDAL(config);
            _user = new UserService(config, httpContextAccessor);
            _dal = _doctorDAL;
        }

        public override int Create(Doctor model)
        {
            if (model.DepartmentId == 0)
                throw new Exception("Department required");

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                int doctorRoleId =_roleDAL.GetRoleByName("Doctor").RoleId;

                // STEP 1: create user
                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = model.Password,
                    RoleId = doctorRoleId,   // Doctor Role
                    ClinicId = model.ClinicId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    IsActive = true,
                    IsFirstLogin = true
                };

                int newUserId = _user.Add(user, conn, tran);

                // STEP 2: create doctor
                var doctor = new Doctor
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DepartmentId = model.DepartmentId,
                    RegistrationNo = model.RegistrationNo,
                    Qualification = model.Qualification,
                    ClinicId = model.ClinicId,
                    Username = model.Username,
                    UserId = newUserId
                };

                int id = base.Add(doctor, conn, tran);

                tran.Commit();
                return id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public override bool Modify(Doctor model)
        {
            // cannot change ClinicId as doctor is tied to a clinic.

            if (model.DepartmentId == 0)
                throw new Exception("Department required");

            int doctorRoleId =_roleDAL.GetRoleByName("Doctor").RoleId;

            var user = new User
            {
                Username = model.Username,
                PasswordHash = model.Password,
                RoleId = doctorRoleId,   // Doctor Role
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                if (_user.Update(user, conn, tran) == false)
                    throw new Exception("Associated user not found");

                bool result = base.Update(model, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public override List<Doctor> List()
        {
            return _doctorDAL.GetDoctorsByClinic(ClinicId);
        }

        public List<Doctor> GetDoctorsByDepartment(int departmentId)
        {
            return _doctorDAL.GetDoctorsByDepartment(ClinicId, departmentId);
        }

        public int GetDoctorIdByUserId(int userId)
        {
            return _doctorDAL.GetDoctorIdByUserId(userId);
        }
    }
}