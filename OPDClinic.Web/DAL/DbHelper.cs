using System.Configuration;
using System.Data.SqlClient;

namespace OPDClinic.Web.DAL
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}