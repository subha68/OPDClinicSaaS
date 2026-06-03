using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Models;
using ViewModels;

namespace DAL
{
    public class VisitDAL : IDAL<Visit>
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

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Visits WHERE VisitId = @VisitId";
            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@VisitId", id);
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Visit> GetAll()
        {
            var list = new List<Visit>();

            using (SqlConnection conn = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Visits", conn);

                conn.Open();
                var reader = cmd.ExecuteReader();

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
            }
            return list;
        }

        public List<Visit> GetVisitsByPatient(int patientId)
        {
            var list = new List<Visit>();
            SqlConnection conn = _db.GetConnection();
            
            string query = "SELECT * FROM Visits WHERE PatientId = @PatientId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PatientId", patientId);

            conn.Open();
            var reader = cmd.ExecuteReader();

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

        public Visit GetByPatientAndDate(int patientId, DateTime date)
        {
            Console.WriteLine(date);
            SqlConnection con = _db.GetConnection();

            string sql = @"SELECT TOP 1 *
                        FROM Visits
                        WHERE PatientId=@PatientId
                        AND CAST(VisitDate as date)=CAST(@VisitDate as date)";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@PatientId", patientId);
            cmd.Parameters.AddWithValue("@VisitDate", date);

            con.Open();
            var reader = cmd.ExecuteReader();

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

        public Visit Get(int visitId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT * FROM Visits WHERE VisitId = @VisitId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@VisitId", visitId);

                conn.Open();
                var reader = cmd.ExecuteReader();

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
            }
            return null; // Not found
        }

        public List<Visit> ListByPatient(int patientId)
        {
            using(SqlConnection con = _db.GetConnection())
            {
                string sql = @"
                    SELECT
                        v.VisitId,
                        v.VisitDate,
                        v.VisitType
                    FROM Visits v
                    WHERE v.PatientId = @PatientId
                    ORDER BY v.VisitDate DESC";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@PatientId", patientId);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

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
}