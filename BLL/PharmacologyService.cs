using DAL;
using Models;
using ViewModels;

namespace BLL
{    
    public class PharmacologyService : BaseService<PharmacologyAdvice>
    {
        private readonly PharmacologyDAL _pharmacologyDAL;

        public PharmacologyService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _pharmacologyDAL = new PharmacologyDAL(config);
            _dal = _pharmacologyDAL;
        }

        public void Create(int consultationId, int appointmentId, PharmacologyAdvice advice)
        {
            _pharmacologyDAL.consultationId = consultationId;
            _pharmacologyDAL.appointmentId = appointmentId;

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                _pharmacologyDAL.Add(advice, conn, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

    }

}