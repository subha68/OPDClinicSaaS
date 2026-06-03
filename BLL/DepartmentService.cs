using DAL;
using Models;

namespace BLL
{
    public class DepartmentService : BaseService<Department>
    {
        private readonly DepartmentDAL _departmentDAL;
        public DepartmentService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(httpContextAccessor)
        {
            _departmentDAL = new DepartmentDAL(config);
            _dal = _departmentDAL;
        }

        public override int Add(Department model)
        {
            model.ClinicId = ClinicId;

            if (string.IsNullOrEmpty(model.Name))
                throw new Exception("Department Name is required");

            return base.Add(model);
        }

        public override List<Department> List()
        {
            return _departmentDAL.GetDepartmentsByClinic(ClinicId);
        }

    }
}