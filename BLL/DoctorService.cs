using DAL;
using Models;
using ViewModels;

namespace BLL
{    public class DoctorService : BaseService<DoctorVM>
    {
        private readonly DoctorDAL _doctorDAL;

        public DoctorService(IConfiguration config)
        {
            _doctorDAL = new DoctorDAL(config);
            _dal = _doctorDAL;
        }

        public override int Add(DoctorVM model)
        {
            model.ClinicId = clinicId;

            if (model.DepartmentId == 0)
                throw new Exception("Department required");

            return _dal.Add(model);
        }

        public override List<DoctorVM> List()
        {
            return _doctorDAL.GetDoctorsByClinic(clinicId);
        }

        public List<DoctorVM> GetDoctorsByDepartment(int clinicId, int departmentId)
        {
            return _doctorDAL.GetDoctorsByDepartment(clinicId, departmentId);
        }

        public int GetDoctorIdByUserId(int userId)
        {
            return _doctorDAL.GetDoctorIdByUserId(userId);
        }
    }
}