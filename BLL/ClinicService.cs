using DAL;
using Models;

namespace BLL
{
    public class ClinicService : BaseService<Clinic>
    {
        private readonly ClinicDAL _clinicDAL;
        private readonly PrescriptionTemplateService _template;
        private readonly TestDefinitionService _testDefinition;
        private readonly VitalDefinitionService _vitalDefinition;

        public ClinicService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _clinicDAL = new ClinicDAL(config);
            _template = new PrescriptionTemplateService(config, httpContextAccessor);
            _testDefinition = new TestDefinitionService(config, httpContextAccessor);
            _vitalDefinition = new VitalDefinitionService(config, httpContextAccessor);
            _dal = _clinicDAL;
        }

        public override int Create(Clinic clinic)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                clinic.ClinicCode = "CLN" + DateTime.Now.Ticks.ToString().Substring(10);
                int clinicId = base.Add(clinic, conn, tran);

                // copy default template for new Clinic
                _template.SyncClinicTemplate(clinicId, conn, tran);

                // clone test definitions
                _testDefinition.CloneMasterDefinitions(clinicId, conn, tran);

                // clone vital definitions
                _vitalDefinition.CloneDefaults(clinicId, conn, tran);
               
                tran.Commit();
                return clinicId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public Clinic ValidateClinicCode(string clinicCode)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _clinicDAL.ValidateClinicCode(clinicCode, conn);
            }
            catch
            {
                throw;
            }

        }

        public int CloneDefaultTemplate()
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                // copy default template for new Clinic
                _template.SyncClinicTemplate(ClinicId, conn, tran);

                tran.Commit();
                return ClinicId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

    }
}