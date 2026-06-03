using DAL;
using Models;
using ViewModels;

namespace BLL
{    
    public class PharmacologyService : BaseService<PharmacologyAdvice>
    {
        private readonly PharmacologyDAL _pharmacologyDAL;

        public PharmacologyService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(httpContextAccessor)
        {
            _pharmacologyDAL = new PharmacologyDAL(config);
            _dal = _pharmacologyDAL;
        }

        public void Add(int consultationId, int appointmentId, PharmacologyAdvice advice)
        {
            _pharmacologyDAL.consultationId = consultationId;
            _pharmacologyDAL.appointmentId = appointmentId;
            _pharmacologyDAL.Add(advice);
        }

    }

}