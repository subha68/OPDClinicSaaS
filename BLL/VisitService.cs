using DAL;
using Models;
using Microsoft.Data.SqlClient;

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

        public override Visit Get(int id)
        {
            Visit model = base.Get(id);
            SetCache(model);

            return model;
        }

        public override int Create(Visit model)
        {
            int id = base.Create(model);
            model.VisitId = id;
            SetCache(model);
            
            return id;
        }

        public List<Visit> GetVisitByPatient(int patientId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var visits = GetVisitByPatient(patientId, conn);

                if (visits != null)
                   SetCache(visits.FirstOrDefault());

                return visits;
            }
            catch
            {
                throw;
            }
        }

        public List<Visit> GetVisitByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var visits = _visitDAL.GetVisitsByPatient(patientId, conn);
            if (visits == null)
                throw new Exception($"Visit with ID '{patientId}' not found.");

            SetCache(visits.FirstOrDefault());

            return visits;
        }


        public Visit GetByPatientAndDate(int patientId, DateTime date)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return GetByPatientAndDate(patientId, date, conn);
            }
            catch
            {
                throw;
            }
        }

        public Visit GetByPatientAndDate(int patientId, DateTime date, SqlConnection conn, SqlTransaction? tran = null)
        {
            return _visitDAL.GetByPatientAndDate(patientId, date, conn, tran);
        }

        private void SetCache(Visit v)
        {
            LastVisitId = v.VisitId;
            LastVisitDate = FormatDate(v.VisitDate);
        }
    }
}