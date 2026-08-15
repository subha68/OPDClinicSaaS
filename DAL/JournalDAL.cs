using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class JournalDAL : IDAL<Journal>
    {
        private readonly DbHelper _db;

        public JournalDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Journal journal, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO JournalHeaders (ClinicId, JournalDate, VoucherType, ReferenceType, ReferenceId, Narration)
                                VALUES (@ClinicId, @JournalDate, @VoucherType, @ReferenceType, @ReferenceId, @Narration);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ClinicId", journal.ClinicId);
            cmd.Parameters.AddWithValue("@JournalDate", journal.JournalDate);
            cmd.Parameters.AddWithValue("@VoucherType", journal.VoucherType);
            cmd.Parameters.AddWithValue("@ReferenceType", journal.ReferenceType);
            cmd.Parameters.AddWithValue("@ReferenceId", journal.ReferenceId);
            cmd.Parameters.AddWithValue("@Narration", journal.Narration);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Journal journal, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE JournalHeaders
                             SET ClinicId = @ClinicId,
                                 JournalDate = @JournalDate,
                                 VoucherType = @VoucherType,
                                 ReferenceType = @ReferenceType,
                                 ReferenceId = @ReferenceId,
                                 Narration = @Narration
                             WHERE JournalLineId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ClinicId", journal.ClinicId);
            cmd.Parameters.AddWithValue("@JournalDate", journal.JournalDate);
            cmd.Parameters.AddWithValue("@VoucherType", journal.VoucherType);
            cmd.Parameters.AddWithValue("@ReferenceType", journal.ReferenceType);
            cmd.Parameters.AddWithValue("@ReferenceId", journal.ReferenceId);
            cmd.Parameters.AddWithValue("@Narration", journal.Narration);
            cmd.Parameters.AddWithValue("@id", journal.JournalHeaderId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM JournalHeaders WHERE JournalHeaderId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Journal> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Journal>();

            var cmd = new SqlCommand("SELECT * FROM JournalHeaders", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Journal
                {
                    JournalHeaderId = (int)reader["JournalHeaderId"],
                    ClinicId = (int)reader["ClinicId"],
                    JournalDate = (DateTime)reader["JournalDate"],
                    VoucherType = reader["VoucherType"].ToString(),
                    ReferenceType = reader["ReferenceType"].ToString(),
                    ReferenceId = (int)reader["ReferenceId"],
                    Narration = reader["Narration"].ToString()                        
                });
            }

            return list;
        }

        public Journal Get(int journalHeaderId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            Journal journal = null;

            var cmd = new SqlCommand("SELECT * FROM JournalHeaders WHERE JournalHeaderId = @JournalHeaderId", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@JournalHeaderId", journalHeaderId);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                journal = new Journal
                {
                    JournalHeaderId = (int)reader["JournalHeaderId"],
                    ClinicId = (int)reader["ClinicId"],
                    JournalDate = (DateTime)reader["JournalDate"],
                    VoucherType = reader["VoucherType"].ToString(),
                    ReferenceType = reader["ReferenceType"].ToString(),
                    ReferenceId = (int)reader["ReferenceId"],
                    Narration = reader["Narration"].ToString()                        
                };
            }

            return journal;
        }

    }
}   