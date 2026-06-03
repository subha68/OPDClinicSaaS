using DAL;
using Models;
using Helpers;

namespace BLL
{
    public abstract class BaseService<T>
    {
        public IDAL<T> _dal;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SessionHelper _session;

        protected BaseService()
        {
        }

        protected BaseService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = new SessionHelper(_httpContextAccessor);
        }

        public int ClinicId => Convert.ToInt32(_session.ClinicId);
        public int UserId => Convert.ToInt32(_session.UserId);
        public string RoleName => _session.RoleName;

        public virtual int Add(T model)
        {
            return _dal.Add(model);
        }

        public virtual bool Update(T model)
        {
            return _dal.Update(model);
        }

        public virtual bool Delete(int id)
        {
            return _dal.Delete(id);
        }

        public virtual T Get(int id)
        {
            return _dal.Get(id);
        }

        public virtual List<T> List()
        {
            return _dal.GetAll();
        }

    }
}