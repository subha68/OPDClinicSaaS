using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Models;
using ViewModels;

namespace DAL
{
    public class VisitDAL : BaseDAL, IDAL<Visit>
    {
        private readonly DbHelper _db;

        public VisitDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public int Add(Visit visit, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Visits (VisitDate, VisitType, ClinicId, PatientId) VALUES (@VisitDate, @VisitType, @ClinicId, @PatientId);
                                    SELECT SCOPE_IDENTITY();";
            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@VisitDate", visit.VisitDate);
            cmd.Parameters.AddWithValue("@VisitType", visit.VisitType);
            cmd.Parameters.AddWithValue("@ClinicId", visit.ClinicId);
            cmd.Parameters.AddWithValue("@PatientId", visit.PatientId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Visit visit, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Visits 
                            SET VisitDate = @VisitDate, VisitType = @VisitType, ClinicId = @ClinicId, PatientId = @PatientId 
                            WHERE VisitId = @VisitId";
            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@VisitDate", visit.VisitDate);
            cmd.Parameters.AddWithValue("@VisitType", visit.VisitType);
            cmd.Parameters.AddWithValue("@ClinicId", visit.ClinicId);
            cmd.Parameters.AddWithValue("@PatientId", visit.PatientId);
            cmd.Parameters.AddWithValue("@VisitId", visit.VisitId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Visits WHERE VisitId = @VisitId";
            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@VisitId", id);
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Visit> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Visit>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Visits", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Visit
                {
                    VisitId = (int)reader["VisitId"],
                    VisitDate = (DateTime)reader["VisitDate"],
                    VisitType = reader["VisitType"].ToString(),
                    ClinicId = (int)reader["ClinicId"],
                    PatientId = (int)reader["PatientId"]
                });
            }

            return list;
        }

        public List<Visit> GetVisitsByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Visit>();
            
            string query = "SELECT v.* FROM Visits v WHERE PatientId = @PatientId ORDER BY v.VisitDate DESC";
            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Visit
                {
                    VisitId = (int)reader["VisitId"],
                    VisitDate = (DateTime)reader["VisitDate"],
                    VisitType = reader["VisitType"].ToString(),
                    ClinicId = (int)reader["ClinicId"],
                    PatientId = (int)reader["PatientId"]
                });
            }

            return list; // Return the list of visits
        }

        public Visit GetByPatientAndDate(int patientId, DateTime date, SqlConnection conn, SqlTransaction? tran = null)
        {
            string sql = @"SELECT TOP 1 *
                        FROM Visits
                        WHERE PatientId=@PatientId
                        AND CAST(VisitDate as date)=CAST(@VisitDate as date)";

            SqlCommand cmd = new SqlCommand(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@VisitDate", date);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Visit
                {
                    VisitId = (int)reader["VisitId"],
                    VisitDate = (DateTime)reader["VisitDate"],
                    VisitType = reader["VisitType"].ToString(),
                    ClinicId = (int)reader["ClinicId"],
                    PatientId = (int)reader["PatientId"]
                };
            }
            
            return null;
        }

        public Visit Get(int visitId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = "SELECT * FROM Visits WHERE VisitId = @VisitId";
            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@VisitId", visitId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Visit
                {
                    VisitId = (int)reader["VisitId"],
                    VisitDate = (DateTime)reader["VisitDate"],
                    VisitType = reader["VisitType"].ToString(),
                    ClinicId = (int)reader["ClinicId"],
                    PatientId = (int)reader["PatientId"]
                };
            }

            return null; // Not found
        }

        public List<Visit> ListByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string sql = @"
                SELECT
                    v.VisitId,
                    v.VisitDate,
                    v.VisitType
                FROM Visits v
                WHERE v.PatientId = @PatientId
                ORDER BY v.VisitDate DESC";

            SqlCommand cmd = new SqlCommand(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using SqlDataReader dr = cmd.ExecuteReader();

            List<Visit> visits = new();

            while(dr.Read())
            {
                visits.Add(new Visit
                {
                    VisitId = Convert.ToInt32(dr["VisitId"]),
                    VisitDate = Convert.ToDateTime(dr["VisitDate"]),
                    VisitType = dr["VisitType"].ToString()
                });
            }

            return visits;
        }

    }
}