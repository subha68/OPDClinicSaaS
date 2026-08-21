using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class CollectionSummaryReportDAL
    {
        private readonly DbHelper _db;

        public CollectionSummaryReportDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public List<DailyCollectionDataRow> GetDailyCollectionData(DailyCollectionSummaryFilter filter,
                                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            List<DailyCollectionDataRow> list = new List<DailyCollectionDataRow>();

            string query = @"
                SELECT  COALESCE(p.CollectionDate, r.CollectionDate) AS CollectionDate,
                        COALESCE(p.PaymentMode, r.PaymentMode) AS PaymentMode,
                        ISNULL(p.TransactionCount, 0) AS TransactionCount,
                        ISNULL(p.CollectionAmount, 0) AS CollectionAmount,
                        ISNULL(r.RefundAmount, 0) AS RefundAmount
                FROM (SELECT    PaymentDate AS CollectionDate,
                                Mode AS PaymentMode,
                                COUNT(PaymentId) AS TransactionCount,
                                SUM(ISNULL(Amount, 0)) AS CollectionAmount
                        FROM Payments
                        WHERE ClinicId = @ClinicId
                            AND PaymentDate >= @DateFrom
                            AND PaymentDate < DATEADD(DAY, 1, @DateTo)
                            AND (@PaymentMode IS NULL
                                    OR @PaymentMode = ''
                                    OR Mode = @PaymentMode)
                        GROUP BY PaymentDate, Mode) as p
                FULL OUTER JOIN  (SELECT    RefundDate AS CollectionDate,
                                            Mode AS PaymentMode,
                                            SUM(ISNULL(Amount, 0)) AS RefundAmount
                                    FROM Refunds
                                    WHERE ClinicId = @ClinicId
                                        AND RefundDate >= @DateFrom
                                        AND RefundDate < DATEADD(DAY, 1, @DateTo)
                                        AND (@PaymentMode IS NULL
                                                OR @PaymentMode = ''
                                                OR Mode = @PaymentMode)
                                    GROUP BY RefundDate, Mode) as r
                    ON p.CollectionDate = r.CollectionDate
                    AND (p.PaymentMode = r.PaymentMode
                            OR (p.PaymentMode IS NULL AND r.PaymentMode IS NULL))
                ORDER BY COALESCE(p.CollectionDate, r.CollectionDate),
                        COALESCE(p.PaymentMode, r.PaymentMode);
            ";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@PaymentMode", string.IsNullOrWhiteSpace(filter.PaymentMode)
                                                                ? DBNull.Value
                                                                : filter.PaymentMode);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new DailyCollectionDataRow
                {
                    CollectionDate = Convert.ToDateTime(rdr["CollectionDate"]),
                    PaymentMode = rdr["PaymentMode"] != DBNull.Value ? rdr["PaymentMode"].ToString() ?? "" : "",
                    TransactionCount = Convert.ToInt32(rdr["TransactionCount"]),
                    CollectionAmount = Convert.ToDecimal(rdr["CollectionAmount"]),
                    RefundAmount = Convert.ToDecimal(rdr["RefundAmount"])
                });
            }

            return list;
        }

        public List<ReportSummaryItem> GetDailyCollectionSummary(DailyCollectionSummaryFilter filter,
                                                                    SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportSummaryItem> list = new();

            string query = @"
                SELECT
                    SUM(ISNULL(p.TransactionCount, 0)) AS TransactionCount,
                    SUM(ISNULL(p.CollectionAmount, 0)) AS CollectionAmount,
                    SUM(ISNULL(r.RefundAmount, 0)) AS RefundAmount,
                    SUM(ISNULL(p.CollectionAmount, 0))
                        - SUM(ISNULL(r.RefundAmount, 0)) AS NetCollection
                FROM
                (
                    SELECT
                        COUNT(PaymentId) AS TransactionCount,
                        SUM(ISNULL(Amount, 0)) AS CollectionAmount
                    FROM Payments
                    WHERE ClinicId = @ClinicId
                    AND PaymentDate >= @DateFrom
                    AND PaymentDate <= @DateTo
                    AND (
                            @PaymentMode IS NULL
                            OR @PaymentMode = ''
                            OR Mode = @PaymentMode
                        )
                ) p
                CROSS JOIN
                (
                    SELECT
                        SUM(ISNULL(Amount, 0)) AS RefundAmount
                    FROM Refunds
                    WHERE ClinicId = @ClinicId
                    AND RefundDate >= @DateFrom
                    AND RefundDate <= @DateTo
                    AND (
                            @PaymentMode IS NULL
                            OR @PaymentMode = ''
                            OR Mode = @PaymentMode
                        )
                ) r;
            ";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", filter.ClinicId);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@DateTo", filter.DateTo.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@PaymentMode", string.IsNullOrWhiteSpace(filter.PaymentMode)
                                                            ? DBNull.Value
                                                            : filter.PaymentMode);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                list.Add(new ReportSummaryItem("Transactions", rdr["TransactionCount"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Collections", rdr["CollectionAmount"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Refunds", rdr["RefundAmount"].ToString() ?? "0"));
                list.Add(new ReportSummaryItem("Net Collection", rdr["NetCollection"].ToString() ?? "0"));
            }

            return list;
        }
    }
}