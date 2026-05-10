using DAL;
using Models;
using ViewModels;

namespace BLL
{    public class DoctorService
    {
        private readonly DoctorDAL _dal;

        public DoctorService(IConfiguration config)
        {
            _dal = new DoctorDAL(config);
        }

        public void CreateDoctor(DoctorVM model, int clinicId)
        {
            model.ClinicId = clinicId;

            if (model.DepartmentId == 0)
                throw new Exception("Department required");

            _dal.AddDoctor(model);
        }

        public List<DoctorVM> GetDoctors(int clinicId)
        {
            return _dal.GetDoctorsByClinic(clinicId);
        }

        public List<DoctorVM> GetDoctorsByDepartment(int clinicId, int departmentId)
        {
            return _dal.GetDoctorsByDepartment(clinicId, departmentId);
        }
    }
}