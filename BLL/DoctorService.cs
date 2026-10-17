using DAL;
using Models;
using ViewModels;
using Helpers;

namespace BLL
{    public class DoctorService : BaseService<Doctor>
    {
        private readonly DoctorDAL _doctorDAL;
        private readonly RoleDAL _roleDAL;
        private readonly DepartmentDAL _departmentDAL;

        private readonly UserService _user;
        private readonly PrescriptionTemplateService _template;

        public DoctorService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _doctorDAL = new DoctorDAL(config);
            _roleDAL = new RoleDAL(config);
            _departmentDAL = new DepartmentDAL(config);
            _user = new UserService(config, httpContextAccessor);
            _template = new PrescriptionTemplateService(config, httpContextAccessor);
            _dal = _doctorDAL;
        }

        public override int Create(Doctor model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                model.ClinicId = ClinicId;

                if (model.DepartmentId == 0)
                    throw new Exception("Department required");

                // check if department belongs to this clinic
                var department = _departmentDAL.Get(model.DepartmentId, ClinicId, conn, tran);

                if (department == null)
                    throw new InvalidOperationException($"Department {model.DepartmentId} not found for clinic {model.ClinicId}.");

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
                model.UserId = newUserId;

                int id = base.Add(model, conn, tran);
                model.DoctorId = id;

                // create template
                int templateId = _template.SyncClinicDoctorTemplate(ClinicId, model.DoctorId, conn, tran);
                model.PrescriptionTemplateId = templateId;

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
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                // cannot change ClinicId as doctor is tied to a clinic.
                model.ClinicId = ClinicId;

                if (model.DepartmentId == 0)
                    throw new Exception("Department required");

                // check if department belongs to this clinic
                var department = _departmentDAL.Get(model.DepartmentId, ClinicId, conn, tran);

                if (department == null)
                    throw new InvalidOperationException($"Department {model.DepartmentId} not found for clinic {model.ClinicId}.");

                int doctorRoleId =_roleDAL.GetRoleByName("Doctor", conn, tran).RoleId;

                var user = new User
                {
                    UserId = model.UserId,
                    Username = model.Username,
                    PasswordHash = model.Password,
                    RoleId = doctorRoleId,   // Doctor Role
                    FirstName = model.Salutation + " " + model.FirstName,
                    LastName = model.LastName
                };

                if (_user.Update(user, conn, tran) == false)
                    throw new Exception("Associated user not found");

                bool result = base.Update(model, conn, tran);

                if (!result)
                    throw new InvalidOperationException(
                        $"Doctor {model.DoctorId} could not be modified for clinic {model.ClinicId}.");


                int templateId = _template.SyncClinicDoctorTemplate(ClinicId, model.DoctorId, conn, tran);
                model.PrescriptionTemplateId = templateId;

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