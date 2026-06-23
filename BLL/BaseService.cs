using DAL;
using Models;
using Helpers;
using Microsoft.Data.SqlClient;

namespace BLL
{
    public abstract class BaseService<T>
    {
        public IDAL<T> _dal;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SessionHelper _session;
        public readonly DbHelper _db;

        protected BaseService()
        {
        }

        protected BaseService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = new SessionHelper(_httpContextAccessor);
            _db = new DbHelper(config);
        }

        public int ClinicId => Convert.ToInt32(_session.ClinicId);
        public int UserId => Convert.ToInt32(_session.UserId);
        public string RoleName => _session.RoleName;

        public virtual int Add(T model, SqlConnection conn, SqlTransaction tran)
        {
            return _dal.Add(model, conn, tran); 
        }

        public virtual bool Update(T model, SqlConnection conn, SqlTransaction tran)
        {
            return _dal.Update(model, conn, tran);
        }

        public virtual bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            return _dal.Delete(id, conn, tran);
        }

        public virtual T Get(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _dal.Get(id, conn);
            }
            catch
            {
                throw;
            }
            
        }

        public virtual List<T> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _dal.GetAll(conn);
            }
            catch
            {
                throw;
            }
            
        }

        public virtual T Get(int id, SqlConnection conn)
        {
            return _dal.Get(id, conn);
        }

        public virtual List<T> List(SqlConnection conn)
        {
            return _dal.GetAll(conn);
        }

        public virtual int Create(T model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                int id = Add(model, conn, tran);
                tran.Commit();
                return id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public virtual bool Modify(T model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                bool result = Update(model, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
               tran.Rollback();
               throw; 
            }
        }


        public virtual bool Remove(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                bool result = Delete(id, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

    }
}