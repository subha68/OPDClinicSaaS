using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public abstract class BaseDAL
    {
        protected int? GetNullableInt(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? null
                : Convert.ToInt32(reader[column]);
        }

        protected decimal? GetNullableDecimal(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? null
                : Convert.ToDecimal(reader[column]);
        }

        protected DateTime? GetNullableDate(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? null
                : Convert.ToDateTime(reader[column]);
        }

        protected string? GetNullableString(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? null
                : reader[column].ToString();
        }

        public string GetNextNo(int clinicId, DateTime date, String instrumentType, SqlConnection conn, SqlTransaction tran)
        {
            // Indian financial year:
            // 01-Apr-2026 to 31-Mar-2027 => FY 2026
            int fy = date.Month >= 4
                ? date.Year
                : date.Year - 1;

            const string selectSql = @"
                SELECT Prefix, FY, InstrumentNo
                FROM InstrumentNoGenerators WITH (UPDLOCK, HOLDLOCK)
                WHERE ClinicId = @ClinicId
                AND InstrumentType = @InstrumentType;";

            var noGenerator = new InstrumentNoGenerator();

            using (var cmd = new SqlCommand(selectSql, conn, tran))
            {
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);
                cmd.Parameters.AddWithValue("@InstrumentType", instrumentType);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            $"Bill number generator is not configured for clinic {clinicId}.");
                    }

                    noGenerator.Prefix = reader["Prefix"]?.ToString();
                    noGenerator.FY = Convert.ToInt32(reader["FY"]);
                    noGenerator.Number = Convert.ToInt64(reader["InstrumentNo"]);
                    noGenerator.InstrumentType = instrumentType;
                }
            }   // reader is definitely closed here


            // ----------------------------------------
            // Generate next number
            // ----------------------------------------

            if (fy == noGenerator.FY)
            {
                noGenerator.Number++;
            }
            else
            {
                // New financial year
                noGenerator.FY = fy;
                noGenerator.Number = 1;
            }


            // ----------------------------------------
            // Persist generator
            // ----------------------------------------

            const string updateSql = @"
                UPDATE InstrumentNoGenerators
                SET
                    FY = @FY,
                    InstrumentNo = @InstrumentNo
                WHERE ClinicId = @ClinicId
                AND InstrumentType = @InstrumentType;";

            using (var updateCmd = new SqlCommand(updateSql, conn, tran))
            {
                updateCmd.Parameters.AddWithValue("@FY", noGenerator.FY);
                updateCmd.Parameters.AddWithValue("@InstrumentNo", noGenerator.Number);
                updateCmd.Parameters.AddWithValue("@InstrumentType", noGenerator.InstrumentType);
                updateCmd.Parameters.AddWithValue("@ClinicId", clinicId);

                int rows = updateCmd.ExecuteNonQuery();

                if (rows != 1)
                {
                    throw new InvalidOperationException(
                        $"Unable to update bill number generator for clinic {clinicId}.");
                }
            }

            return noGenerator.InstrumentNo;
        }
    }
}