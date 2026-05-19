using System.Diagnostics;

namespace DAL
{
    public interface IDAL<T>
    {
        int Add(T model);
        bool Update(T model);
        bool Delete(int id);
        T Get(int id);
        List<T> GetAll();
    }
}