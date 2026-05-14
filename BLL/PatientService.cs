using DAL;
using Models;
using ViewModels;

namespace BLL
{    public class PatientService
    {
        private readonly PatientDAL _dal;

        public PatientService(IConfiguration config)
        {
            _dal = new PatientDAL(config);
        }

        public Patient GetByMobile(string mobile)
        {
            return _dal.GetByMobile(mobile);
        }

    }

}

