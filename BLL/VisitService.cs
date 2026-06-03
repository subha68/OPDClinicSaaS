using DAL;
using Models;

namespace BLL
{
    public class VisitService : BaseService<Visit>
    {
        private readonly VisitDAL _visitDAL;

        public VisitService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _visitDAL = new VisitDAL(config);
            _dal = _visitDAL;
        }

        public List<Visit> GetVisitByPatient(int patientId)
        {
            var visits = _visitDAL.GetVisitsByPatient(patientId);
            if (visits == null)
                throw new Exception($"Visit with ID '{patientId}' not found.");

            return visits;
        }

        public Visit GetByPatientAndDate(int patientId, DateTime date)
        {
            return _visitDAL.GetByPatientAndDate(patientId, date);
        }

    }
}