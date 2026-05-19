using DAL;
using Models;

namespace BLL
{
    public abstract class BaseService<T>
    {
        public IDAL<T> _dal;
        public int clinicId = 0;

        public void setClinicId(int id)
        {
            clinicId = id;
        }

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