using DAL;
using Models;

namespace BLL
{
    public class ClinicService : BaseService<Clinic>
    {
        private readonly ClinicDAL _clinicDAL;

        public ClinicService(IConfiguration config)
        {
            _clinicDAL = new ClinicDAL(config);
            _dal = _clinicDAL;
        }

        public override int Add(Clinic clinic)
        {
            clinic.ClinicCode = "CLN" + DateTime.Now.Ticks.ToString().Substring(10);
            return base.Add(clinic);
        }

        public Clinic ValidateClinicCode(string clinicCode)
        {
            return _clinicDAL.ValidateClinicCode(clinicCode);
        }

        public override List<Clinic> List()
        {
            return base.List();
        }

        public Clinic Get(int clinicId)
        {
            return _dal.Get(clinicId);
        }
    }
}