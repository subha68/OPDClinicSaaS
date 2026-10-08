using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class AccountGroupDAL : BaseDAL, IDAL<AccountGroup>
    {
        private readonly DbHelper _db;

        public AccountGroupDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(AccountGroup accountGroup, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO AccountGroups (ClinicId, GroupName, ParentGroupId, Nature)
                                VALUES (@ClinicId, @GroupName, @ParentGroupId, @Nature);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ClinicId", accountGroup.ClinicId);
            cmd.Parameters.AddWithValue("@GroupName", accountGroup.GroupName);
            cmd.Parameters.AddWithValue("@ParentGroupId", accountGroup.ParentGroupId);
            cmd.Parameters.AddWithValue("@Nature", accountGroup.Nature);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(AccountGroup accountGroup, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE AccountGroups
                             SET ClinicId = @ClinicId,
                                 GroupName = @GroupName,
                                 ParentGroupId = @ParentGroupId,
                                 Nature = @Nature
                             WHERE AccountGroupId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ClinicId", accountGroup.ClinicId);
            cmd.Parameters.AddWithValue("@GroupName", accountGroup.GroupName);
            cmd.Parameters.AddWithValue("@ParentGroupId", accountGroup.ParentGroupId);
            cmd.Parameters.AddWithValue("@Nature", accountGroup.Nature);
            cmd.Parameters.AddWithValue("@id", accountGroup.AccountGroupId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM AccountGroups WHERE ClinicId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<AccountGroup> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<AccountGroup>();

            var cmd = new SqlCommand("SELECT * FROM AccountGroups", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new AccountGroup
                {
                    AccountGroupId = (int)reader["AccountGroupId"],
                    ClinicId = (int)reader["ClinicId"],
                    GroupName = reader["GroupName"].ToString(),
                    ParentGroupId = (int)reader["ParentGroupId"],
                    Nature = reader["Nature"].ToString()
                });
            }

            return list;
        }

        public AccountGroup Get(int accountGroupId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            AccountGroup accountGroup = null;

            var cmd = new SqlCommand("SELECT * FROM AccuntGroups WHERE AccountGroupId = @accountGroupId", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@accountGroupId", accountGroupId);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                accountGroup = new AccountGroup
                {
                    AccountGroupId = (int)reader["AccountGroupId"],
                    ClinicId = (int)reader["ClinicId"],
                    GroupName = reader["GroupName"].ToString(),
                    ParentGroupId = (int)reader["ParentGroupId"],
                    Nature = reader["Nature"].ToString()
                };
            }

            return accountGroup;
        }

    }
}   