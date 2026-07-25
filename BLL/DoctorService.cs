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
                int doctorRoleId =_roleDAL.GetRoleByName("Doctor", conn, tran).RoleId;

                // STEP 1: create user
                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = model.Password,
                    RoleId = doctorRoleId,   // Doctor Role
                    ClinicId = model.ClinicId,
                    FirstName = model.Salutation + " " + model.FirstName,
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
                    Salutation = model.Salutation,
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

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                int doctorRoleId =_roleDAL.GetRoleByName("Doctor", conn, tran).RoleId;

                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = model.Password,
                    RoleId = doctorRoleId,   // Doctor Role
                    FirstName = model.Salutation + " " + model.FirstName,
                    LastName = model.LastName
                };

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
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _doctorDAL.GetDoctorsByClinic(ClinicId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<Doctor> GetDoctorsByDepartment(int departmentId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _doctorDAL.GetDoctorsByDepartment(ClinicId, departmentId, conn);
            }
            catch
            {
                throw;
            }

        }

        public int GetDoctorIdByUserId(int userId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _doctorDAL.GetDoctorIdByUserId(userId, conn);
            }
            catch
            {
                throw;
            }

        }
    }
}