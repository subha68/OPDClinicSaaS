using DAL;
using Models;
using ViewModels;

namespace BLL
{
    public class DepartmentService : BaseService<DepartmentVM>
    {
        private readonly DepartmentDAL _departmentDAL;
        public DepartmentService(IConfiguration config)
        {
            _departmentDAL = new DepartmentDAL(config);
            _dal = _departmentDAL;
        }

        public override int Add(DepartmentVM model)
        {
            model.ClinicId = clinicId;

            if (string.IsNullOrEmpty(model.Name))
                throw new Exception("Department Name is required");

            return base.Add(model);
        }

        public override List<DepartmentVM> List()
        {
            return _departmentDAL.GetDepartmentsByClinic(clinicId);
        }

    }
}