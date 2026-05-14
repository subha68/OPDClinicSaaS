using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection string is not configured.");

            // To connect to SQLServer Express from Command Line, use:
            // sqlcmd -S LENOVO-SUBHASIS\SQLEXPRESS -No -E
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}