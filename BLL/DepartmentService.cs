using DAL;
using Models;
using ViewModels;

namespace BLL
{
    public class DepartmentService
    {
        private readonly DepartmentDAL _dal;
        public DepartmentService(IConfiguration config)
        {
            _dal = new DepartmentDAL(config);
        }

        public void CreateDepartment(DepartmentVM model, int clinicId)
        {
            model.ClinicId = clinicId;

            if (string.IsNullOrEmpty(model.Name))
                throw new Exception("Department Name is required");

            _dal.AddDepartment(model);
        }

        public List<DepartmentVM> GetDepartments(int clinicId)
        {
            return _dal.GetDepartmentsByClinic(clinicId);
        }

    }
}