using DAL;
using Models;

namespace BLL
{
    public class ClinicService
    {
        private readonly ClinicDAL _dal;

        public ClinicService(IConfiguration config)
        {
            _dal = new ClinicDAL(config);
        }

        public void CreateClinic(Clinic clinic)
        {
            clinic.ClinicCode = "CLN" + DateTime.Now.Ticks.ToString().Substring(10);
            _dal.AddClinic(clinic);
        }

        public List<Clinic> GetClinics()
        {
            return _dal.GetClinics();
        }
    }
}