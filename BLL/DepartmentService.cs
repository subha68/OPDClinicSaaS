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
        private readonly PrescriptionTemplateDAL _templateDAL;
        public DepartmentService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _departmentDAL = new DepartmentDAL(config);
            _templateDAL = new PrescriptionTemplateDAL(config);
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
                int departmentId = Add(model, conn, tran);

                if (model.ToBeSourceTemplateId != 0)
                {
                    int templateId = _templateDAL.CloneTemplate((int)model.ToBeSourceTemplateId, 0, ClinicId, departmentId, conn, tran);
                    model.PrescriptionTemplateId = templateId;
               }

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

            if (string.IsNullOrEmpty(model.Name))
                throw new Exception("Department Name is required");

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                bool result = Update(model, conn, tran);

                if (model.ToBeSourceTemplateId != model.SourceTemplateId)
                {
                    int templateId = ReplaceDepartmentTemplate(model.DepartmentId, (int)model.ToBeSourceTemplateId,  
                                                                                conn, tran);
                    model.PrescriptionTemplateId = templateId;
                    model.SourceTemplateId = model.ToBeSourceTemplateId;
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

        public int ReplaceDepartmentTemplate(int departmentId, int toBeSourceTemplateId, 
                                                        SqlConnection conn, SqlTransaction tran)
        {
            if (departmentId <= 0)
                return 0;

            PrescriptionTemplate template = _templateDAL.GetByDepartment(departmentId, ClinicId, conn, tran);

            if (template == null)
                return 0;

            if (toBeSourceTemplateId == template.SourceTemplateId)
                // no change required
                return template.TemplateId;

            _templateDAL.Delete(template.TemplateId, ClinicId, conn, tran);

            if (toBeSourceTemplateId == 0)
                // No new template should be associated.
                // Simply delete the existing department-specific template.
                return 0;

            // Replace existing department-specific template
            // with a clone of the selected source template.
            int newTemplateId = _templateDAL.CloneTemplate(toBeSourceTemplateId, 0, ClinicId, departmentId, conn, tran);

            return newTemplateId;                
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