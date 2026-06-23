using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public interface IDAL<T>
    {
        int Add(T model, SqlConnection conn, SqlTransaction tran);
        bool Update(T model, SqlConnection conn, SqlTransaction tran);
        bool Delete(int id, SqlConnection conn, SqlTransaction tran);
        T Get(int id, SqlConnection conn, SqlTransaction? tran = null);
        List<T> GetAll(SqlConnection conn, SqlTransaction? tran = null);
    }
}