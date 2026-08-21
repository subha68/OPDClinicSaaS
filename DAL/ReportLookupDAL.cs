using Microsoft.Data.SqlClient;
using Models;

namespace DAL
{
    public class ReportLookupDAL
    {
        private readonly DbHelper _db;

        public ReportLookupDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public List<ReportLookupItem> GetPaymentModes(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportLookupItem> list = new List<ReportLookupItem>();

            string query = @"
                SELECT DISTINCT Mode
                FROM Payments
                WHERE ClinicId = @ClinicId
                  AND Mode IS NOT NULL
                  AND LTRIM(RTRIM(Mode)) <> ''

                UNION

                SELECT DISTINCT Mode
                FROM Refunds
                WHERE ClinicId = @ClinicId
                  AND Mode IS NOT NULL
                  AND LTRIM(RTRIM(Mode)) <> ''

                ORDER BY Mode;
            ";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                string mode = rdr["Mode"].ToString() ?? "";

                list.Add(new ReportLookupItem
                {
                    Value = mode,
                    Text = mode
                });
            }

            return list;
        }
    }
}