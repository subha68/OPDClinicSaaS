using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public interface IDAL<T>
    {
        int Add(T model, SqlConnection conn, SqlTransaction tran);
        bool Update(T model, SqlConnection conn, SqlTransaction tran);
        bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran);
        T Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null);
        List<T> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null);

    }
}