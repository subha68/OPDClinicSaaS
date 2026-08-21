using DAL;
using Models;

namespace BLL
{
    public class ClinicService : BaseService<Clinic>
    {
        private readonly ClinicDAL _clinicDAL;
        private readonly PrescriptionTemplateDAL _templateDAL;

        public ClinicService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _clinicDAL = new ClinicDAL(config);
            _templateDAL = new PrescriptionTemplateDAL(config);
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
                _templateDAL.CloneTemplate("DEFAULT", 0, clinicId, 0, conn, tran);

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
                _templateDAL.CloneTemplate("DEFAULT", 0, ClinicId, 0, conn, tran);

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