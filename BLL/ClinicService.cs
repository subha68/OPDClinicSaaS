using DAL;
using Models;

namespace BLL
{
    public class ClinicService : BaseService<Clinic>
    {
        private readonly ClinicDAL _clinicDAL;

        public ClinicService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _clinicDAL = new ClinicDAL(config);
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
                int id = base.Add(clinic, conn, tran);
                tran.Commit();
                return id;
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

    }
}