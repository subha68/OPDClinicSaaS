using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class JournalLineDAL : IDAL<JournalLine>
    {
        private readonly DbHelper _db;

        public JournalLineDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(JournalLine journalLine, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO JournalLines (ClinicId, JournalHeaderId, LedgerAccountId, DebitAmount, CreditAmount)
                                VALUES (@ClinicId, @JournalHeaderId, @LedgerAccountId, @DebitAmount, @CreditAmount);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ClinicId", journalLine.ClinicId);
            cmd.Parameters.AddWithValue("@JournalHeaderId", journalLine.JournalHeaderId);
            cmd.Parameters.AddWithValue("@LedgerAccountId", journalLine.LedgerAccountId);
            cmd.Parameters.AddWithValue("@DebitAmount", journalLine.DebitAmount);
            cmd.Parameters.AddWithValue("@CreditAmount", journalLine.CreditAmount);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(JournalLine journalLine, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE JournalLines
                             SET ClinicId = @ClinicId,
                                 JournalHeaderId = @JournalHeaderId,
                                 LedgerAccountId = @LedgerAccountId,
                                 DebitAmount = @DebitAmount,
                                 CreditAmount = @CreditAmount
                             WHERE JournalLineId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ClinicId", journalLine.ClinicId);
            cmd.Parameters.AddWithValue("@JournalHeaderId", journalLine.JournalHeaderId);
            cmd.Parameters.AddWithValue("@LedgerAccountId", journalLine.LedgerAccountId);
            cmd.Parameters.AddWithValue("@DebitAmount", journalLine.DebitAmount);
            cmd.Parameters.AddWithValue("@CreditAmount", journalLine.CreditAmount);
            cmd.Parameters.AddWithValue("@id", journalLine.JournalLineId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM JournalLines WHERE JournalLineId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<JournalLine> GetAll(SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<JournalLine>();

            var cmd = new SqlCommand("SELECT * FROM JournalLines", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new JournalLine
                {
                    JournalLineId = (int)reader["JournalLineId"],
                    ClinicId = (int)reader["ClinicId"],
                    JournalHeaderId = (int)reader["JournalHeaderId"],
                    LedgerAccountId = (int)reader["LedgerAccountId"],
                    DebitAmount = Convert.ToDecimal(reader["DebitAmount"]),
                    CreditAmount = Convert.ToDecimal(reader["ReferenceId"])                        
                });
            }

            return list;
        }

        public JournalLine Get(int journalLineId, SqlConnection conn, SqlTransaction? tran = null)
        {
            JournalLine journalLine = null;

            var cmd = new SqlCommand("SELECT * FROM JournalLines WHERE JournalLineId = @JournalLineId", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@JournalLineId", journalLineId);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                journalLine = new JournalLine
                {
                    JournalLineId = (int)reader["JournalLineId"],
                    ClinicId = (int)reader["ClinicId"],
                    JournalHeaderId = (int)reader["JournalHeaderId"],
                    LedgerAccountId = (int)reader["LedgerAccountId"],
                    DebitAmount = Convert.ToDecimal(reader["DebitAmount"]),
                    CreditAmount = Convert.ToDecimal(reader["ReferenceId"])                        
                };
            }

            return journalLine;
        }

    }
}   