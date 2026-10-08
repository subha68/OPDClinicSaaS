using Microsoft.Data.SqlClient;
using Models;

namespace DAL
{
    public class ReportLookupDAL : BaseDAL
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

        public List<ReportLookupItem> SearchDoctors(int clinicId, string? term, SqlConnection conn, SqlTransaction? tran = null)
        {
            var result = new List<ReportLookupItem>();

            if (string.IsNullOrWhiteSpace(term))
            {
                return result;
            }

            const string sql = @"
                SELECT DoctorId, Salutation + ' ' + FirstName + ' ' + LastName as DoctorName
                FROM Doctors
                WHERE ClinicId = @ClinicId
                AND FirstName + ' ' + LastName LIKE '%' + @SearchText + '%'
                ORDER BY FirstName, LastName;";

            using var cmd = conn.CreateCommand();

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@SearchText", term ?? "");

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new ReportLookupItem
                {
                    Value = reader["DoctorId"].ToString()!,
                    Text = reader["DoctorName"].ToString()!
                });
            }

            return result;
        }

        public List<ReportLookupItem> SearchPatients(int clinicId, string? term, SqlConnection conn, SqlTransaction? tran = null)
        {
            var result = new List<ReportLookupItem>();

            if (string.IsNullOrWhiteSpace(term))
            {
                return result;
            }

            const string sql = @"
                SELECT PatientId, FirstName + ' ' + LastName as PatientName
                FROM Patients
                WHERE ClinicId = @ClinicId
                AND FirstName + ' ' + LastName LIKE '%' + @SearchText + '%'
                ORDER BY FirstName, LastName;";

            using var cmd = conn.CreateCommand();

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@SearchText", term ?? "");

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new ReportLookupItem
                {
                    Value = reader["PatientId"].ToString()!,
                    Text = reader["PatientName"].ToString()!
                });
            }

            return result;
        }

        public string? GetDoctorName(int clinicId, int doctorId, SqlConnection conn, SqlTransaction? tran = null)
        {
            const string sql = @"
                SELECT Salutation + ' ' + FirstName + ' ' + LastName AS DoctorName
                FROM Doctors
                WHERE ClinicId = @ClinicId
                AND DoctorId = @DoctorId;";

            using var cmd = conn.CreateCommand();

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@DoctorId", doctorId);

            object? result = cmd.ExecuteScalar();

            return result == null || result == DBNull.Value ? null : result.ToString();
        }

        public string? GetPatientName(int clinicId, int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            const string sql = @"
                SELECT FirstName + ' ' + LastName AS PatientName
                FROM Patients
                WHERE ClinicId = @ClinicId
                AND PatientId = @PatientId;";

            using var cmd = conn.CreateCommand();

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@PatientId", patientId);

            object? result = cmd.ExecuteScalar();

            return result == null || result == DBNull.Value ? null : result.ToString();
        }

        public List<ReportLookupItem> GetCancelledNoshowAppointmentStatus(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ReportLookupItem> list = new List<ReportLookupItem>();

            string query = @"
                SELECT DISTINCT Status
                FROM Appointments
                WHERE ClinicId = @ClinicId
                  AND Status IS NOT NULL
                  AND LTRIM(RTRIM(Status)) <> ''
                  AND Status IN ('Cancelled', 'No-show')
                ORDER BY Status;
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
                string status = rdr["Status"].ToString() ?? "";

                list.Add(new ReportLookupItem
                {
                    Value = status,
                    Text = status
                });
            }

            return list;
        }

    }
}