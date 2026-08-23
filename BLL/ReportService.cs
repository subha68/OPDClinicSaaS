using DAL;
using Models;
using ViewModels;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using System.IO;

namespace BLL
{
    public class ReportService : BaseService<Report>
    {
        private readonly ReportDAL _reportDAL;
        private readonly ReportLookupDAL _reportLookupDAL;
        private readonly IConfiguration _config;

        public ReportService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _reportDAL = new ReportDAL(config);
            _reportLookupDAL = new ReportLookupDAL(config);
            _config = config;
        }

        public ReportDefinitionVM? GetReportDefinition(string reportCode, SqlConnection conn = null)
        {
            if (conn == null)
            {
                conn = _db.GetConnection();                
                conn.Open();
            }

            if (string.IsNullOrWhiteSpace(reportCode))
            {
                return null;
            }

            Report? report = _reportDAL.Get(reportCode.Trim(), conn);

            if (report == null || !report.IsActive)
            {
                return null;
            }

            List<ReportColumn> columns = _reportDAL.GetColumns(report.ReportId, conn);
            List<ReportFilter> filters = _reportDAL.GetFilters(report.ReportId, conn);

            return new ReportDefinitionVM
            {
                Report = report,
                Columns = columns,
                Filters = filters,
                CanExport = HasReportPermission(report.ReportId, ReportPermissionCodes.Export, conn),
                CanPrint = HasReportPermission(report.ReportId, ReportPermissionCodes.Print, conn)
            };
        }

        public ReportDataSourceResultVM ExecuteReport(string reportCode, Dictionary<string, object?> parameters)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                ReportDefinitionVM? definition = GetReportDefinition(reportCode, conn);

                if (definition == null)
                {
                    throw new InvalidOperationException($"Report '{reportCode}' was not found or is inactive.");
                }

                ReportExecutionDefinition? executionDefinition = ReportRegistry.Get(reportCode);

                if (executionDefinition == null)
                {
                    throw new InvalidOperationException($"No execution definition registered for report '{reportCode}'.");
                }

                object filter = BuildFilter(definition, executionDefinition, parameters);

                object dal = Activator.CreateInstance(executionDefinition.DalType, _config)!;

                object? result = InvokeReportMethod(executionDefinition.DalType, executionDefinition.MethodName,
                                                        dal, filter, conn);

                ReportDataSourceResultVM reportResult = BuildResult(definition, result, executionDefinition.RowType);

                // populate the report context
                reportResult.ReportTitle = definition.Report.ReportName;
                reportResult.ReportCode = definition.Report.ReportCode;
                reportResult.GeneratedOn = DateTime.Now;

                // populate the filters
                foreach (ReportFilter reportFilter in definition.Filters)
                {
                    parameters.TryGetValue(reportFilter.ParameterName, out object? value);

                    if (value == null && !string.IsNullOrWhiteSpace(reportFilter.DefaultValue))
                    {
                        value = reportFilter.DefaultValue;
                    }

                    object? displayValue = GetFilterDisplayValue(reportFilter, value, conn);

                    reportResult.Filters.Add(new ReportResultFilterVM
                    {
                        FilterCode = reportFilter.FilterCode,
                        FilterLabel = reportFilter.FilterLabel,
                        Value = displayValue
                    });
                }

                // Populate report summary
                if (!string.IsNullOrWhiteSpace(executionDefinition.SummaryMethodName))
                {
                    object? summaryResult = InvokeReportMethod(executionDefinition.DalType, executionDefinition.SummaryMethodName,
                                                                dal, filter, conn);

                    if (summaryResult is not List<ReportSummaryItem> summary)
                    {
                        throw new InvalidOperationException(
                            $"Summary method '{executionDefinition.SummaryMethodName}' " +
                            $"on DAL '{executionDefinition.DalType.Name}' " +
                            $"must return List<ReportSummaryItem>.");
                    }

                    reportResult.Summary = summary;
                }

                return reportResult;

            }
            catch
            {
                throw;
            }
        }

        private object? GetFilterDisplayValue(ReportFilter filter, object? value, SqlConnection conn)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (IsEmptyValue(value))
            {
                return null;
            }

            string controlType = (filter.ControlType ?? "")
                                    .Trim()
                                    .ToLowerInvariant();

            if (controlType == "lookup" && !string.IsNullOrWhiteSpace(filter.LookupCode))
            {
                switch (filter.LookupCode.Trim().ToUpperInvariant())
                {
                    case "DOCTOR":
                        if (int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int doctorId))
                        {
                            return _reportLookupDAL.GetDoctorName(ClinicId, doctorId, conn);
                        }
                        break;

                    case "PATIENT":
                        if (int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int patientId))
                        {
                            return _reportLookupDAL.GetDoctorName(ClinicId, patientId, conn);
                        }
                        break;

                    default:
                        break;
                }
            }

            return value;
        }

        private object? InvokeReportMethod(Type dalType, string methodName, object dal, object filter, SqlConnection conn)
        {
            MethodInfo? method = dalType.GetMethod(methodName);

            if (method == null)
            {
                throw new InvalidOperationException(
                    $"Method '{methodName}' was not found on DAL " +
                    $"'{dalType.Name}'.");
            }

            return method.Invoke(dal,
                                    new object?[]
                                    {
                                        filter,
                                        conn,
                                        null
                                    });
        }

        private string NormalizeName(string value)
        {
            return value
                .Replace("_", "")
                .ToLowerInvariant();
        }

        private bool IsEmptyValue(object? value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is string text)
            {
                return string.IsNullOrWhiteSpace(text);
            }

            return false;
        } 

        private object? ConvertValue(object? value, Type targetType)
        {
            if (value == null || value == DBNull.Value)
            {
                if (Nullable.GetUnderlyingType(targetType) != null || !targetType.IsValueType)
                {
                    return null;
                }

                return Activator.CreateInstance(targetType);
            }

            // Empty form values should be treated as null.
            // This is important for optional filters such as DoctorId.
            if (value is string textStr && string.IsNullOrWhiteSpace(textStr))
            {
                return null;
            }

            Type actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Already the correct type
            if (actualType.IsInstanceOfType(value))
            {
                return value;
            }

            if (actualType == typeof(string))
            {
                return value.ToString();
            }

            if (actualType == typeof(DateTime))
            {
                return FormatDateTime(value);
            }

            if (actualType == typeof(DateOnly))
            {
                return FormatDate(value);
            }

            if (actualType == typeof(TimeSpan))
            {
                return FormatTime(value);
            }

            if (actualType == typeof(decimal))
            {
                return FormatDecimal(value);
            }

            if (actualType == typeof(int))
            {
                return FormatInteger(value);
            }

            if (actualType == typeof(long))
            {
                return FormatLong(value);
            }

            if (actualType == typeof(short))
            {
                return FormatShort(value);
            }

            if (actualType == typeof(bool))
            {
                return FormatBoolean(value);
            }

            if (actualType.IsEnum)
            {
                return Enum.Parse(actualType, value.ToString()!, true);
            }

            return Convert.ChangeType(value, actualType, CultureInfo.InvariantCulture);
        }              
        
        private object BuildFilter(ReportDefinitionVM definition, ReportExecutionDefinition executionDefinition, Dictionary<string, object?> parameters)
        {
            object filter = Activator.CreateInstance(executionDefinition.FilterType)
                                                            ?? throw new InvalidOperationException(
                                                                $"Unable to create filter model " +
                                                                $"'{executionDefinition.FilterType.Name}'.");

            PropertyInfo[] properties = executionDefinition.FilterType.GetProperties(
                                            BindingFlags.Public | BindingFlags.Instance);

            foreach (ReportFilter reportFilter in definition.Filters)
            {
                string parameterName = reportFilter.ParameterName;

                parameters.TryGetValue(parameterName, out object? value);

                // Use metadata default value if no value was supplied
                if (value == null && !string.IsNullOrWhiteSpace(reportFilter.DefaultValue))
                {
                    value = reportFilter.DefaultValue;
                }

                // Required validation
                if (reportFilter.IsRequired && IsEmptyValue(value))
                {
                    throw new ArgumentException($"Parameter '{parameterName}' is required " +
                                                    $"for report '{definition.Report.ReportCode}'.");
                }

                // Find corresponding filter model property.
                // Example:
                // date_from  -> DateFrom
                // payment_mode -> PaymentMode
                string normalizedParameter = NormalizeName(parameterName);

                PropertyInfo? property = properties.FirstOrDefault(p =>
                                                        NormalizeName(p.Name) == normalizedParameter);

                if (property == null)
                {
                    throw new InvalidOperationException($"Filter parameter '{parameterName}' does not " +
                                                            $"match any property on filter model " +
                                                            $"'{executionDefinition.FilterType.Name}'.");
                }

                object? convertedValue = ConvertValue(value, property.PropertyType);

                // Validate according to the rule configured
                // in reporting.report_filter.
                ValidateFilter(definition, reportFilter, convertedValue, parameters);

                property.SetValue(filter, convertedValue);
            }

            // ClinicId comes from application context,
            // not from report metadata.
            PropertyInfo? clinicProperty = properties.FirstOrDefault(p =>NormalizeName(p.Name) == "clinicid");

            if (clinicProperty != null)
            {
                clinicProperty.SetValue(filter, ConvertValue(ClinicId, clinicProperty.PropertyType));
            }

            return filter;
        }

        private ReportDataSourceResultVM BuildResult(ReportDefinitionVM definition, object? result, Type rowType)
        {
            ReportDataSourceResultVM reportResult = new();

            // Build column metadata for the client.
            foreach (ReportColumn column in definition.Columns
                                                    .Where(c => c.IsVisible)
                                                    .OrderBy(c => c.DisplayOrder))
            {
                reportResult.Columns.Add(new ReportResultColumn
                {
                    ColumnCode = column.ColumnCode,
                    DisplayLabel = column.DisplayLabel,
                    DataType = column.DataType,
                    DisplayOrder = column.DisplayOrder,
                    FormatPattern = column.FormatPattern
                });
            }
    
            if (result == null)
            {
                return reportResult;
            }

            if (result is not System.Collections.IEnumerable rows)
            {
                throw new InvalidOperationException(
                    $"Report DAL result must be an enumerable collection " +
                    $"of '{rowType.Name}'.");
            }

            PropertyInfo[] properties = rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Build normalized property map once.
            Dictionary<string, PropertyInfo> propertyMap = new(StringComparer.OrdinalIgnoreCase);

            foreach (PropertyInfo property in properties)
            {
                string normalizedName = NormalizeName(property.Name);

                if (propertyMap.ContainsKey(normalizedName))
                {
                    throw new InvalidOperationException(
                        $"Multiple properties on '{rowType.Name}' " +
                        $"resolve to the same normalized name " +
                        $"'{normalizedName}'.");
                }

                propertyMap.Add(normalizedName, property);
            }

            foreach (object? row in rows)
            {
                if (row == null)
                {
                    continue;
                }

                Dictionary<string, object?> outputRow = new(StringComparer.OrdinalIgnoreCase);

                foreach (ReportColumn column in definition.Columns)
                {
                    if (!column.IsVisible)
                    {
                        continue;
                    }

                    string normalizedColumn = NormalizeName(column.ColumnCode);

                    if (!propertyMap.TryGetValue(normalizedColumn, out PropertyInfo? property))
                    {
                        throw new InvalidOperationException(
                            $"Report column '{column.ColumnCode}' does not " +
                            $"match any property on row model " +
                            $"'{rowType.Name}'.");
                    }

                    object? value = property.GetValue(row);

                    outputRow[column.ColumnCode] = FormatReportValue(value, column.DataType, column.FormatPattern);
                }

                reportResult.Rows.Add(outputRow);
            }

            return reportResult;
        }

        private void ValidateFilter(ReportDefinitionVM definition, ReportFilter filter, object? value,
                                        Dictionary<string, object?> parameters)
        {
            if (filter.IsRequired && IsEmptyValue(value))
            {
                throw new ArgumentException($"Parameter '{filter.FilterLabel}' is required.");
            }

            if (IsEmptyValue(value))
            {
                return;
            }

            string rule = filter.ValidationRule?.Trim().ToLowerInvariant() ?? "";

            if (string.IsNullOrEmpty(rule))
            {
                return;
            }

            switch (rule)
            {
                case "date":
                    ValidateDate(filter, value);
                    break;

                case "number":
                    ValidateNumber(filter, value);
                    break;

                case "integer":
                    ValidateInteger(filter, value);
                    break;

                case "decimal":
                    ValidateDecimal(filter, value);
                    break;

                case "date_before_or_equal":
                    ValidateDateBeforeOrEqual(
                        definition,
                        filter,
                        value,
                        parameters);
                    break;

                case "date_after_or_equal":
                    ValidateDateAfterOrEqual(
                        definition,
                        filter,
                        value,
                        parameters);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unknown validation rule '{filter.ValidationRule}' " +
                        $"configured for filter '{filter.FilterCode}'.");
            }
        }

        private void ValidateDate(ReportFilter filter, object? value)
        {
            if (value == null)
            {
                return;
            }

            if (value is DateOnly)
            {
                return;
            }

            if (value is DateTime)
            {
                return;
            }

            if (DateOnly.TryParseExact(value.ToString(),
                                        new[]
                                        {
                                            "yyyy-MM-dd",
                                            "dd-MM-yyyy",
                                            "dd/MM/yyyy"
                                        },
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out _))
            {
                return;
            }

            throw new ArgumentException($"'{filter.FilterLabel}' must be a valid date.");
        }

        private void ValidateNumber(ReportFilter filter, object value)
        {
            if (!decimal.TryParse(value.ToString(),
                                    NumberStyles.Number,
                                    CultureInfo.InvariantCulture,
                                    out _))
            {
                throw new ArgumentException($"'{filter.FilterLabel}' must be a valid number.");
            }
        }

        private void ValidateInteger(ReportFilter filter, object value)
        {
            if (!long.TryParse(value.ToString(),
                                NumberStyles.Integer,
                                CultureInfo.InvariantCulture,
                                out _))
            {
                throw new ArgumentException($"'{filter.FilterLabel}' must be a valid integer.");
            }
        }

        private void ValidateDecimal(ReportFilter filter, object value)
        {
            if (!decimal.TryParse(value.ToString(),
                                    NumberStyles.Number,
                                    CultureInfo.InvariantCulture,
                                    out _))
            {
                throw new ArgumentException($"'{filter.FilterLabel}' must be a valid decimal value.");
            }
        }

        private void ValidateDateBeforeOrEqual(
            ReportDefinitionVM definition,
            ReportFilter filter,
            object? value,
            Dictionary<string, object?> parameters)
        {
            if (value == null)
            {
                return;
            }

            DateOnly fromDate;

            if (value is DateOnly dateOnly)
            {
                fromDate = dateOnly;
            }
            else if (!DateOnly.TryParseExact(
                        value.ToString(),
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out fromDate))
            {
                throw new ArgumentException(
                    $"'{filter.FilterLabel}' must be a valid date.");
            }

            if (!parameters.TryGetValue(
                    "date_to",
                    out object? toValue))
            {
                return;
            }

            if (toValue == null ||
                string.IsNullOrWhiteSpace(toValue.ToString()))
            {
                return;
            }

            DateOnly toDate;

            if (toValue is DateOnly toDateOnly)
            {
                toDate = toDateOnly;
            }
            else if (!DateOnly.TryParseExact(
                        toValue.ToString(),
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out toDate))
            {
                throw new ArgumentException(
                    "'To Date' must be a valid date.");
            }

            if (fromDate > toDate)
            {
                throw new ArgumentException(
                    $"'{filter.FilterLabel}' must be less than " +
                    $"or equal to 'To Date'.");
            }
        }

        private void ValidateDateAfterOrEqual(ReportDefinitionVM definition, ReportFilter filter, object? value,
                                                Dictionary<string, object?> parameters)
        {
            if (value == null)
            {
                return;
            }

            if (value is not DateOnly toDate)
            {
                throw new ArgumentException($"'{filter.FilterLabel}' must be a valid date.");
            }

            // Identify the date-from parameter.
            // For a standard From/To pair this will normally be date_from.
            string fromParameterName = "date_from";

            if (!parameters.TryGetValue(fromParameterName, out object? fromValue))
            {
                return;
            }

            if (fromValue == null)
            {
                return;
            }

            DateOnly fromDate;

            if (fromValue is DateOnly fromDateOnly)
            {
                fromDate = fromDateOnly;
            }
            else
            {
                string text = fromValue.ToString() ?? "";

                if (!DateOnly.TryParseExact(text,
                                            new[]
                                            {
                                                "yyyy-MM-dd",
                                                "dd-MM-yyyy",
                                                "dd/MM/yyyy"
                                            },
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None,
                                            out fromDate))
                {
                    throw new ArgumentException("'From Date' must be a valid date.");
                }
            }

            if (toDate < fromDate)
            {
                throw new ArgumentException($"'{filter.FilterLabel}' must be greater than " +
                                            $"or equal to 'From Date'.");
            }
        }

        public bool HasReportPermission(long reportId, string permissionCode, SqlConnection conn)
        {
            if (reportId <= 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                return false;
            }

            if (_reportDAL.GetPermission(
                    reportId,
                    "user",
                    UserId,
                    permissionCode,
                    conn) != null)
            {
                return true;
            }

            if (_reportDAL.GetPermission(
                    reportId,
                    "role",
                    RoleId,
                    permissionCode,
                    conn) != null)
            {
                return true;
            }

            return false;
        }

        public bool HasReportPermission(long reportId, string permissionCode)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return HasReportPermission(reportId, permissionCode, conn);
        }        

        public bool CanExport(long reportId, SqlConnection conn)
        {
            return HasReportPermission(reportId, ReportPermissionCodes.Export, conn);
        }

        public bool CanExport(long reportId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return CanExport(reportId, conn);
        }

        public bool CanPrint(long reportId, SqlConnection conn)
        {
            return HasReportPermission(reportId, ReportPermissionCodes.Print, conn);
        }

        public bool CanPrint(long reportId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return CanPrint(reportId, conn);
        }

        public void EnsureExportPermission(long reportId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            if (!CanExport(reportId, conn))
            {
                throw new UnauthorizedAccessException("You do not have permission to export this report.");
            }
        }

        public byte[] ExportCsv(long reportId, string reportCode, Dictionary<string, object?> parameters)
        {
            // ============================================================
            // PERMISSION
            // ============================================================

            EnsureExportPermission(reportId);


            // ============================================================
            // EXECUTE REPORT
            // ============================================================

            ReportDataSourceResultVM result = ExecuteReport(reportCode, parameters);

            // ============================================================
            // BUILD CSV
            // ============================================================

            var csv = new StringBuilder();


            // ------------------------------------------------------------
            // HEADER
            // ------------------------------------------------------------

            var columns = result.Columns
                                .OrderBy(c => c.DisplayOrder)
                                .ToList();

            csv.AppendLine(string.Join(",",
                                        columns.Select(c =>
                                            EscapeCsv(c.DisplayLabel ?? c.ColumnCode))));


            // ------------------------------------------------------------
            // DATA
            // ------------------------------------------------------------

            foreach (var row in result.Rows)
            {
                var values = columns.Select(column =>
                {
                    object? value = GetRowValue(row, column.ColumnCode);

                    return EscapeCsv(FormatReportValue(value, column.DataType, column.FormatPattern));
                });

                csv.AppendLine(
                    string.Join(",", values));
            }


            // ============================================================
            // UTF-8 BOM
            // ============================================================

            return new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: true)
                .GetBytes(csv.ToString());
        }

        private string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            if (value.Contains('"') ||
                value.Contains(',') ||
                value.Contains('\r') ||
                value.Contains('\n'))
            {
                return "\"" +
                    value.Replace("\"", "\"\"") +
                    "\"";
            }

            return value;
        }

        private object? GetRowValue(
            object row,
            string columnCode)
        {
            if (row == null)
            {
                return null;
            }

            if (row is IDictionary<string, object?> dictionary)
            {
                if (dictionary.TryGetValue(
                        columnCode,
                        out object? value))
                {
                    return value;
                }

                string normalizedColumn =
                    NormalizeName(columnCode);

                string? key =
                    dictionary.Keys.FirstOrDefault(
                        k => NormalizeName(k) == normalizedColumn);

                return key != null
                    ? dictionary[key]
                    : null;
            }

            return null;
        }

        public byte[] ExportExcel(long reportId, string reportCode, Dictionary<string, object?> parameters)
        {
            // ============================================================
            // PERMISSION
            // ============================================================

            EnsureExportPermission(reportId);

            // ============================================================
            // EXECUTE REPORT
            // ============================================================

            ReportDataSourceResultVM result = ExecuteReport(reportCode, parameters);

            // ============================================================
            // CREATE WORKBOOK
            // ============================================================

            using var workbook = new XLWorkbook();

            IXLWorksheet worksheet = workbook.Worksheets.Add("Report");

            // ============================================================
            // REPORT TITLE
            // ============================================================

            int currentRow = 1;

            worksheet.Cell(currentRow, 1).Value = result.ReportTitle ?? result.ReportCode ?? "Report";
            worksheet.Range(currentRow, 1, currentRow, 6).Merge();
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            currentRow += 2;

            // ============================================================
            // GENERATED DATE
            // ============================================================

            worksheet.Cell(currentRow, 1).Value = "Generated On";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = result.GeneratedOn;
            worksheet.Cell(currentRow, 2).Style.DateFormat.Format = "dd-MMM-yyyy HH:mm";

            currentRow += 2;

            // ============================================================
            // FILTERS
            // ============================================================

            if (result.Filters.Any())
            {
                worksheet.Cell(currentRow, 1).Value = "Filters";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;

                currentRow++;

                foreach (ReportResultFilterVM filter in result.Filters)
                {
                    worksheet.Cell(currentRow, 1).Value = filter.FilterLabel;
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = FormatReportValue(filter.Value);

                    currentRow++;
                }

                currentRow++;
            }


            // ============================================================
            // SUMMARY
            // ============================================================

            if (result.Summary.Any())
            {
                worksheet.Cell(currentRow, 1).Value = "Summary";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;

                currentRow++;

                foreach (ReportSummaryItem summary in result.Summary)
                {
                    worksheet.Cell(currentRow, 1).Value = summary.Label;
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Value = summary.Value;

                    currentRow++;
                }

                currentRow += 2;
            }


            // ============================================================
            // TABULAR DATA
            // ============================================================

            List<ReportResultColumn> columns = result.Columns
                                                    .OrderBy(c => c.DisplayOrder)
                                                    .ToList();

            int headerRow = currentRow;

            for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                ReportResultColumn column = columns[columnIndex];
                IXLCell cell = worksheet.Cell(headerRow, columnIndex + 1);
                cell.Value = column.DisplayLabel ?? column.ColumnCode;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }


            // ============================================================
            // DATA ROWS
            // ============================================================

            int dataRow = headerRow + 1;

            foreach (Dictionary<string, object?> row in result.Rows)
            {
                for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    ReportResultColumn column = columns[columnIndex];
                    object? value = GetRowValue(row, column.ColumnCode);
                    IXLCell cell = worksheet.Cell(dataRow, columnIndex + 1);
                    SetExcelCellValue(cell, value, column);
                }

                dataRow++;
            }


            // ============================================================
            // FORMAT TABLE
            // ============================================================

            if (columns.Count > 0)
            {
                int lastDataRow = Math.Max(headerRow, dataRow - 1);
                IXLRange tableRange = worksheet.Range(headerRow, 1, lastDataRow, columns.Count);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.OutsideBorderColor = XLColor.LightGray;
                tableRange.Style.Border.InsideBorderColor = XLColor.LightGray;

                // Freeze the report header.
                // worksheet.SheetView.FreezeRows(headerRow);

                // Enable Excel filtering.
                worksheet.Range(headerRow, 1, lastDataRow, columns.Count)
                        .SetAutoFilter();
            }


            // ============================================================
            // COLUMN WIDTHS
            // ============================================================

            worksheet.Columns().AdjustToContents();

            // Keep excessively wide columns under control.
            foreach (IXLColumn column in worksheet.ColumnsUsed())
            {
                if (column.Width > 40)
                {
                    column.Width = 40;
                }
            }


            // ============================================================
            // OUTPUT
            // ============================================================

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        private string FormatReportValue(object? value, string? dataType = null, string? formatPattern = null)
        {
            if (value == null || value == DBNull.Value)
            {
                return "";
            }

            string type = NormalizeDataType(dataType);

            switch (type)
            {
                case "date":
                    return FormatDate(value).ToString(formatPattern ?? "dd-MM-yyyy", CultureInfo.InvariantCulture);

                case "datetime":
                    return FormatDateTime(value).ToString(formatPattern ?? "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);

                case "time":
                    return FormatTime(value).ToString(formatPattern ?? @"hh\:mm", CultureInfo.InvariantCulture);

                case "decimal":
                case "number":
                    return FormatDecimal(value).ToString(CultureInfo.InvariantCulture);

                case "integer":
                    return FormatInteger(value).ToString(CultureInfo.InvariantCulture);

                case "boolean":
                    return FormatBoolean(value) ? "Yes" : "No";

                default:
                    return FormatGeneralValue(value, formatPattern);
            }
        }

        private void SetExcelCellValue(IXLCell cell, object? value, ReportResultColumn column)
        {
            if (value == null || value == DBNull.Value)
            {
                cell.Value = "";
                return;
            }

            string type = NormalizeDataType(column.DataType);

            switch (type)
            {
                case "date":
                    cell.Value = FormatDate(value).ToDateTime(TimeOnly.MinValue);
                    cell.Style.DateFormat.Format = string.IsNullOrWhiteSpace(column.FormatPattern)
                                                                            ? "dd-MMM-yyyy"
                                                                            : column.FormatPattern;
                    return;

                case "datetime":
                    cell.Value = FormatDateTime(value);
                    cell.Style.DateFormat.Format = string.IsNullOrWhiteSpace(column.FormatPattern)
                                                                            ? "dd-MMM-yyyy HH:mm"
                                                                            : column.FormatPattern;
                    return;

                case "time":
                    cell.Value = FormatTime(value).TotalDays;
                    cell.Style.NumberFormat.Format = string.IsNullOrWhiteSpace(column.FormatPattern)
                                                                                ? "HH:mm"
                                                                                : column.FormatPattern;
                    return;

                case "decimal":
                case "number":
                    cell.Value = FormatDecimal(value);
                    cell.Style.NumberFormat.Format = string.IsNullOrWhiteSpace(column.FormatPattern)
                                                                                ? "#,##0.00"
                                                                                : column.FormatPattern;                    
                    return;

                case "integer":
                    cell.Value = FormatInteger(value);
                    cell.Style.NumberFormat.Format = string.IsNullOrWhiteSpace(column.FormatPattern)
                                                                                ? "#,##0"
                                                                                : column.FormatPattern;
                    return;

                case "boolean":
                    cell.Value = FormatBoolean(value) ? "Yes" : "No";
                    return;

                default:
                    cell.Value = FormatGeneralValue(value, column.FormatPattern);
                    return;
            }
        }

    }
}