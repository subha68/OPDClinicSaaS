using DAL;
using Models;
using ViewModels;

namespace BLL
{    
    public class PatientService : BaseService<Patient>
    {
        private readonly PatientDAL _patientDAL;

        public PatientService(IConfiguration config)
        {
            _patientDAL = new PatientDAL(config);
            _dal = _patientDAL;
        }

        public Patient GetByMobile(string mobile)
        {
            return _patientDAL.GetByMobile(mobile);
        }

    }

}

