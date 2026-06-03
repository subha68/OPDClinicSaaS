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
                                : base(httpContextAccessor)
        {
            _doctorDAL = new DoctorDAL(config);
            _roleDAL = new RoleDAL(config);
            _user = new UserService(config, httpContextAccessor);
            _dal = _doctorDAL;
        }

        public override int Add(Doctor model)
        {
            if (model.DepartmentId == 0)
                throw new Exception("Department required");

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

            int newUserId = _user.Add(user);

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

            return base.Add(doctor);
        }

        public override bool Update(Doctor model)
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


            if (_user.Update(user) == false)
                throw new Exception("Associated user not found");

            return base.Update(model);
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