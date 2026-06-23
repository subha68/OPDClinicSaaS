using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class LedgerAccountDAL : IDAL<LedgerAccount>
    {
        private readonly DbHelper _db;

        public LedgerAccountDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(LedgerAccount ledgerAccount, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO LedgerAccounts (ClinicId, AccountGroupId, AccountCode, AccountName, IsActive)
                                VALUES (@ClinicId, @AccountGroupId, @AccountCode, @AccountName, @IsActive);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ClinicId", ledgerAccount.ClinicId);
            cmd.Parameters.AddWithValue("@AccountGroupId", ledgerAccount.AccountGroupId);
            cmd.Parameters.AddWithValue("@AccountCode", ledgerAccount.AccountCode);
            cmd.Parameters.AddWithValue("@AccountName", ledgerAccount.AccountName);
            cmd.Parameters.AddWithValue("@IsActive", ledgerAccount.IsActive);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(LedgerAccount ledgerAccount, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE LedgerAccounts
                             SET ClinicId = @ClinicId,
                                 AccountGroupId = @AccountGroupId,
                                 AccountCode = @AccountCode,
                                 AccountName = @AccountName,
                                 IsActive = @IsActive
                             WHERE LedgerAccountId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ClinicId", ledgerAccount.ClinicId);
            cmd.Parameters.AddWithValue("@AccountGroupId", ledgerAccount.AccountGroupId);
            cmd.Parameters.AddWithValue("@AccountCode", ledgerAccount.AccountCode);
            cmd.Parameters.AddWithValue("@AccountName", ledgerAccount.AccountName);
            cmd.Parameters.AddWithValue("@IsActive", ledgerAccount.IsActive);
            cmd.Parameters.AddWithValue("@id", ledgerAccount.LedgerAccountId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM LedgerAccounts WHERE LedgerAccountId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<LedgerAccount> GetAll(SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<LedgerAccount>();

            var cmd = new SqlCommand("SELECT * FROM LedgerAccounts", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new LedgerAccount
                {
                    LedgerAccountId = (int)reader["LedgerAccountId"],
                    ClinicId = (int)reader["ClinicId"],
                    AccountGroupId = (int)reader["AccountGroupId"],
                    AccountCode = reader["AccountCode"].ToString(),
                    AccountName = reader["AccountName"].ToString(),
                    IsActive = reader["IsActive"].ToString()
                });
            }

            return list;
        }

        public LedgerAccount Get(int ledgerAccountId, SqlConnection conn, SqlTransaction? tran = null)
        {
            LedgerAccount ledgerAccount = null;

            var cmd = new SqlCommand("SELECT * FROM LedgerAccounts WHERE LedgerAccountId = @LedgerAccountId", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@LedgerAccountId", ledgerAccountId);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                ledgerAccount = new LedgerAccount
                {
                    LedgerAccountId = (int)reader["LedgerAccountId"],
                    ClinicId = (int)reader["ClinicId"],
                    AccountGroupId = (int)reader["AccountGroupId"],
                    AccountCode = reader["AccountCode"].ToString(),
                    AccountName = reader["AccountName"].ToString(),
                    IsActive = reader["IsActive"].ToString()
                };
            }

            return ledgerAccount;
        }

        public int GetId(string accountCode, SqlConnection conn, SqlTransaction? tran = null)
        {
            var cmd = new SqlCommand("SELECT LedgerAccountId FROM LedgerAccounts WHERE AccountCode = @AccountCode", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@AccountCode", accountCode);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return (int)reader["LedgerAccountId"];
            }

            return 0;
        }


    }
}   