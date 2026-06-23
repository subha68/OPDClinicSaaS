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
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var visits = _visitDAL.GetVisitsByPatient(patientId, conn);
                if (visits == null)
                    throw new Exception($"Visit with ID '{patientId}' not found.");

                return visits;
            }
            catch
            {
                throw;
            }
        }

        public Visit GetByPatientAndDate(int patientId, DateTime date)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _visitDAL.GetByPatientAndDate(patientId, date, conn);
            }
            catch
            {
                throw;
            }
        }

    }
}