using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class ReportDAL : BaseDAL
    {
        private readonly DbHelper _db;

        public ReportDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public Report? Get(string reportCode, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT report_id, report_code, report_name, description, data_source_code, report_type, is_active
                FROM reporting.report
                WHERE report_code = @ReportCode
                  AND is_active = 1;
            ";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ReportCode", reportCode);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                return new Report
                {
                    ReportId = Convert.ToInt64(rdr["report_id"]),
                    ReportCode = rdr["report_code"].ToString() ?? "",
                    ReportName = rdr["report_name"].ToString() ?? "",
                    Description = rdr["description"] != DBNull.Value
                                                            ? rdr["description"].ToString()
                                                            : null,
                    DataSourceCode = rdr["data_source_code"].ToString() ?? "",
                    ReportType = rdr["report_type"].ToString() ?? "",
                    IsActive = Convert.ToBoolean(rdr["is_active"])
                };
            }

            return null;
        }

        public List<ReportColumn> GetColumns(long reportId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportColumn> list = new List<ReportColumn>();

            string query = @"
                SELECT report_column_id, report_id, column_code, display_label, data_type, display_order, is_visible, format_pattern
                FROM reporting.report_column
                WHERE report_id = @ReportId
                ORDER BY display_order;
            ";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ReportId", reportId);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new ReportColumn
                {
                    ReportColumnId = Convert.ToInt64(rdr["report_column_id"]),
                    ReportId = Convert.ToInt64(rdr["report_id"]),
                    ColumnCode = rdr["column_code"].ToString() ?? "",
                    DisplayLabel = rdr["display_label"].ToString() ?? "",
                    DataType = rdr["data_type"].ToString() ?? "",
                    DisplayOrder = Convert.ToInt32(rdr["display_order"]),
                    IsVisible = Convert.ToBoolean(rdr["is_visible"]),
                    FormatPattern = rdr["format_pattern"] != DBNull.Value
                                                                ? rdr["format_pattern"].ToString()
                                                                : null
                });
            }

            return list;
        }

        public List<ReportFilter> GetFilters(long reportId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportFilter> list = new();

            string query = @"
                SELECT report_filter_id, report_id, filter_code, filter_label, parameter_name, data_type,
                        control_type, lookup_code, default_value, is_required, display_order, validation_rule, validation_value
                FROM reporting.report_filter
                WHERE report_id = @ReportId
                ORDER BY display_order;
            ";

            SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ReportId", reportId);

            using SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new ReportFilter
                {
                    ReportFilterId = Convert.ToInt64(rdr["report_filter_id"]),
                    ReportId = Convert.ToInt64(rdr["report_id"]),
                    FilterCode = rdr["filter_code"].ToString() ?? "",
                    FilterLabel = rdr["filter_label"].ToString() ?? "",
                    ParameterName = rdr["parameter_name"].ToString() ?? "",
                    DataType = rdr["data_type"].ToString() ?? "",
                    ControlType = rdr["control_type"].ToString() ?? "",
                    LookupCode = rdr["lookup_code"].ToString() ?? "",
                    DefaultValue = rdr["default_value"] != DBNull.Value
                                                            ? rdr["default_value"].ToString()
                                                            : null,
                    IsRequired = Convert.ToBoolean(rdr["is_required"]),
                    DisplayOrder = Convert.ToInt32(rdr["display_order"]),
                    ValidationRule = rdr["validation_rule"] != DBNull.Value
                                        ? rdr["validation_rule"].ToString()
                                        : null,
                    ValidationValue = rdr["validation_value"] != DBNull.Value
                                        ? rdr["validation_value"].ToString()
                                        : null
                });
            }

            return list;
        } 

        public ReportPermission? GetPermission(long reportId, string principalType, long principalId, string permissionCode,
                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            const string query = @"
                SELECT  report_permission_id,
                        report_id,
                        principal_type,
                        principal_id,
                        permission_code
                FROM reporting.report_permission
                WHERE report_id = @ReportId
                AND principal_type = @PrincipalType
                AND principal_id = @PrincipalId
                AND permission_code = @PermissionCode;
            ";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ReportId", reportId);
            cmd.Parameters.AddWithValue("@PrincipalType", principalType);
            cmd.Parameters.AddWithValue("@PrincipalId", principalId);
            cmd.Parameters.AddWithValue("@PermissionCode", permissionCode);

            using SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                return new ReportPermission
                {
                    ReportPermissionId = Convert.ToInt64(rdr["report_permission_id"]),
                    ReportId = Convert.ToInt64(rdr["report_id"]),
                    PrincipalType = rdr["principal_type"]?.ToString() ?? string.Empty,
                    PrincipalId = Convert.ToInt64(rdr["principal_id"]),
                    PermissionCode = rdr["permission_code"]?.ToString() ?? string.Empty
                };
            }

            return null;

        }      

    }
}