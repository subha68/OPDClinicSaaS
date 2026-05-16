using DAL;
using Models;
using ViewModels;

namespace BLL
{    
    public class PharmacologyService
    {
        private readonly PharmacologyDAL _dal;

        public PharmacologyService(IConfiguration config)
        {
            _dal = new PharmacologyDAL(config);
        }

        public void Add(int appointmentId, PharmacologyAdviceVM advice)
        {
            _dal.Add(appointmentId, advice);
        }

    }

}