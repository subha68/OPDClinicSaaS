using DAL;
using Models;

namespace BLL
{
    public class DepartmentService : BaseService<Department>
    {
        private readonly DepartmentDAL _departmentDAL;
        public DepartmentService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _departmentDAL = new DepartmentDAL(config);
            _dal = _departmentDAL;
        }

        public override int Create(Department model)
        {
            model.ClinicId = ClinicId;

            if (string.IsNullOrEmpty(model.Name))
                throw new Exception("Department Name is required");

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                int id = Add(model, conn, tran);
                tran.Commit();
                return id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public override List<Department> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _departmentDAL.GetDepartmentsByClinic(ClinicId, conn);
            }
            catch
            {
                throw;
            }

        }

    }
}