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
        private readonly PrescriptionTemplateDAL _templateDAL;

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
                if (model.CloneTemplate && 
                    _template.GetByDepartment(model.DepartmentId, model.ClinicId, conn, tran) != null)
                {
                    int templateId = _template.SyncClinicDoctorTemplate(ClinicId, model.DoctorId, conn, tran);
                    model.PrescriptionTemplateId = templateId;
                }

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
                // Doctor remains tied to the current clinic.
                model.ClinicId = ClinicId;

                if (model.DepartmentId == 0)
                    throw new InvalidOperationException("Department required.");

                // Validate that the department belongs to this clinic.
                var department = _departmentDAL.Get(model.DepartmentId, ClinicId, conn, tran);

                if (department == null)
                {
                    throw new InvalidOperationException($"Department {model.DepartmentId} not found for clinic {ClinicId}.");
                }

                // Retrieve the existing associated User.
                var user = _user.Get(model.UserId);

                if (user == null)
                    throw new InvalidOperationException("Associated user not found.");

                int doctorRoleId = _roleDAL.GetRoleByName("Doctor", conn, tran).RoleId;

                user.Username = model.Username;

                // IMPORTANT:
                // Blank means retain the existing password.
                // A supplied password will be hashed by UserService.Update().
                user.PasswordHash = model.Password;

                user.RoleId = doctorRoleId;
                user.FirstName = $"{model.Salutation} {model.FirstName}".Trim();
                user.LastName = model.LastName;

                if (!_user.Update(user, conn, tran))
                {
                    throw new InvalidOperationException(
                        $"Associated user {model.UserId} could not be updated.");
                }

                var existingDoctor = _doctorDAL.Get(model.DoctorId, model.ClinicId, conn, tran);
                bool departmentChanged = existingDoctor.DepartmentId != model.DepartmentId;

                bool result = base.Update(model, conn, tran);

                if (!result)
                {
                    throw new InvalidOperationException(
                        $"Doctor {model.DoctorId} could not be modified for clinic {ClinicId}.");
                }

                var doctorTemplate = _template.GetByDoctor(model.DoctorId, model.ClinicId, conn, tran);
                var departmentTemplate = _template.GetByDepartment(model.DepartmentId, model.ClinicId, conn, tran);

                // Clone the department template if requested and the doctor
                // does not already have a template.
                if (model.CloneTemplate && departmentTemplate != null)
                {
                    if (doctorTemplate == null || departmentChanged)
                    {
                        int templateId = _template.SyncClinicDoctorTemplate(ClinicId, model.DoctorId, conn, tran);
                        model.PrescriptionTemplateId = templateId;
                    }
                }
                // Remove the doctor's template if cloning is no longer requested.
                else if (!model.CloneTemplate && doctorTemplate != null)
                {
                    bool deleted = _templateDAL.Delete(doctorTemplate.TemplateId, model.ClinicId, conn, tran);

                    if (!deleted)
                    {
                        throw new InvalidOperationException(
                            $"Could not remove prescription template {doctorTemplate.TemplateId} " +
                            $"for doctor {model.DoctorId}.");
                    }

                    model.PrescriptionTemplateId = null;

                    bool updated = _doctorDAL.Update(model, conn, tran);

                    if (!updated)
                    {
                        throw new InvalidOperationException(
                            $"Could not clear the prescription template reference " +
                            $"for doctor {model.DoctorId}.");
                    }
                }

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