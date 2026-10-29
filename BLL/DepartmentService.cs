using DAL;
using Models;
using Microsoft.Extensions.Configuration;

namespace BLL
{
    public class DepartmentService : BaseService<Department>
    {
        private readonly DepartmentDAL _departmentDAL;
        private readonly PrescriptionTemplateDAL _templateDAL;
        private readonly PrescriptionTemplateService _template;

        public DepartmentService(
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor)
            : base(config, httpContextAccessor)
        {
            _departmentDAL = new DepartmentDAL(config);
            _templateDAL = new PrescriptionTemplateDAL(config);
            _template = new PrescriptionTemplateService(config, httpContextAccessor);
            _dal = _departmentDAL;
        }

        public override int Create(Department model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.ClinicId = ClinicId;
            ValidateAndNormalize(model);

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                EnsureCodeIsUnique(model.DepartmentCode, null, conn, tran);

                int? departmentId = Add(model, conn, tran);

                if (!departmentId.HasValue || departmentId.Value <= 0)
                    throw new InvalidOperationException("Failed to create department.");

                model.DepartmentId = departmentId.Value;

                // A department must have a code, so create/synchronize its
                // clinic-level department template in the same transaction.
                int templateId = _template.SyncClinicDepartmentTemplate(
                    ClinicId, model.DepartmentId, conn, tran);

                if (templateId <= 0)
                    throw new InvalidOperationException(
                        $"Could not create a prescription template for department {model.DepartmentId}.");

                model.PrescriptionTemplateId = templateId;

                tran.Commit();
                return departmentId.Value;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public override bool Modify(Department model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.ClinicId = ClinicId;
            ValidateAndNormalize(model);

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                // Verify tenant ownership before modifying anything.
                var existing = _departmentDAL.Get(
                    model.DepartmentId, ClinicId, conn, tran);

                if (existing == null || existing.ClinicId != ClinicId)
                    throw new InvalidOperationException(
                        $"Department {model.DepartmentId} was not found for this clinic.");

                EnsureCodeIsUnique(
                    model.DepartmentCode,
                    model.DepartmentId,
                    conn,
                    tran);

                string oldDepartmentCode = existing.DepartmentCode ?? string.Empty;
                bool codeChanged = !string.Equals(
                    oldDepartmentCode,
                    model.DepartmentCode,
                    StringComparison.OrdinalIgnoreCase);

                // Persist the new code before synchronizing the template,
                // because template-code generation uses the department code.
                bool updated = Update(model, conn, tran);

                if (!updated)
                    throw new InvalidOperationException(
                        $"Department {model.DepartmentId} could not be updated.");

                if (codeChanged)
                {
                    int? templateId = _template.SyncClinicDepartmentTemplate(
                        ClinicId, model.DepartmentId, conn, tran);

                    if (!templateId.HasValue || templateId.Value <= 0)
                        throw new InvalidOperationException(
                            $"Could not synchronize the prescription template for department {model.DepartmentId}.");

                    var template = _templateDAL.Get(
                        templateId.Value, ClinicId, conn, tran);

                    if (template == null)
                        throw new InvalidOperationException(
                            $"Could not retrieve the prescription template for department {model.DepartmentId}.");

                    model.PrescriptionTemplateId = templateId.Value;
                    model.PrescriptionTemplateName = template.TemplateName;

                    // Store the synchronized template association on the department.
                    updated = _departmentDAL.Update(model, conn, tran);

                    if (!updated)
                        throw new InvalidOperationException(
                            $"Could not save the prescription-template association for department {model.DepartmentId}.");
                }
                else
                {
                    model.PrescriptionTemplateId = existing.PrescriptionTemplateId;
                    model.PrescriptionTemplateName = existing.PrescriptionTemplateName;
                }

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        private static void ValidateAndNormalize(Department model)
        {
            model.DepartmentCode = model.DepartmentCode?.Trim() ?? string.Empty;
            model.Name = model.Name?.Trim() ?? string.Empty;
            model.Description = model.Description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(model.DepartmentCode))
                throw new InvalidOperationException("Department Code is required.");

            if (model.DepartmentCode.Length > 30)
                throw new InvalidOperationException(
                    "Department Code cannot exceed 30 characters.");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Department Name is required.");
        }

        private void EnsureCodeIsUnique(
            string departmentCode,
            int? departmentId,
            Microsoft.Data.SqlClient.SqlConnection conn,
            Microsoft.Data.SqlClient.SqlTransaction tran)
        {
            int count = _departmentDAL.CountByCode(
                ClinicId, departmentId, departmentCode, conn, tran);

            if (count > 0)
                throw new InvalidOperationException(
                    $"Department already exists with code '{departmentCode}'.");
        }

        public override List<Department> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();
            return _departmentDAL.GetDepartmentsByClinic(ClinicId, conn);
        }

        public List<string> ListDepartmentCodes()
        {
            using var conn = _db.GetConnection();
            conn.Open();
            return _templateDAL.ListTemplateCodes(conn);
        }
    }
}
