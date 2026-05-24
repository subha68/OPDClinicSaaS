using DAL;
using Models;
using ViewModels;

namespace BLL
{    public class DoctorService : BaseService<Doctor>
    {
        private readonly DoctorDAL _doctorDAL;

        public DoctorService(IConfiguration config)
        {
            _doctorDAL = new DoctorDAL(config);
            _dal = _doctorDAL;
        }

        public override int Add(Doctor model)
        {
            if (model.DepartmentId == 0)
                throw new Exception("Department required");

            return base.Add(model);
        }

        public override bool Update(Doctor model)
        {
            // cannot change ClinicId as doctor is tied to a clinic.

            if (model.DepartmentId == 0)
                throw new Exception("Department required");

            return base.Update(model);
        }

        public override List<Doctor> List()
        {
            return _doctorDAL.GetDoctorsByClinic(clinicId);
        }

        public List<Doctor> GetDoctorsByDepartment(int clinicId, int departmentId)
        {
            return _doctorDAL.GetDoctorsByDepartment(clinicId, departmentId);
        }

        public int GetDoctorIdByUserId(int userId)
        {
            return _doctorDAL.GetDoctorIdByUserId(userId);
        }
    }
}