using DAL;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace BLL
{
    public class DepartmentService : BaseService<Department>
    {
        private readonly DepartmentDAL _departmentDAL;
        private readonly PrescriptionTemplateService _template;
        public DepartmentService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _departmentDAL = new DepartmentDAL(config);
            _template = new PrescriptionTemplateService(config, httpContextAccessor);
            _dal = _departmentDAL;
        }

        public override int Create(Department model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                model.ClinicId = ClinicId;

                if (string.IsNullOrWhiteSpace(model.Name))
                    throw new Exception("Department Name is required");

                if (string.IsNullOrWhiteSpace(model.DepartmentCode))
                    throw new InvalidOperationException("Department Code is required");

                int count = _departmentDAL.CountByCode(ClinicId, null, model.DepartmentCode, conn, tran);

                if (count > 0)
                    throw new InvalidOperationException($"Department already exists with code {model.DepartmentCode}");

                int departmentId = Add(model, conn, tran);

                int templateId = _template.SyncClinicDepartmentTemplate(ClinicId, departmentId, conn, tran);
                model.PrescriptionTemplateId = templateId;

                tran.Commit();
                return departmentId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public override bool Modify(Department model)
        {
            model.ClinicId = ClinicId;

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    throw new Exception("Department Name is required");

                if (string.IsNullOrWhiteSpace(model.DepartmentCode))
                    throw new InvalidOperationException("Department Code is required");

                int count = _departmentDAL.CountByCode(ClinicId, model.DepartmentId, model.DepartmentCode, conn, tran);

                if (count > 0)
                    throw new InvalidOperationException($"Department already exists with code {model.DepartmentCode}");

                // check if department belongs to this clinic
                var department = _departmentDAL.Get(model.DepartmentId, ClinicId, conn, tran);

                if (department == null || department.ClinicId != ClinicId)
                    throw new InvalidOperationException($"Department {model.DepartmentId} not found for clinic {model.ClinicId}.");

                bool result = Update(model, conn, tran);

                if (!result)
                    throw new InvalidOperationException(
                        $"Department {model.DepartmentId} could not be modified for clinic {model.ClinicId}.");

                int templateId = _template.SyncClinicDepartmentTemplate(ClinicId, model.DepartmentId, conn, tran);
                model.PrescriptionTemplateId = templateId;

                tran.Commit();
                return true;
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