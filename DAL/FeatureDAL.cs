using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class FeatureDAL : IDAL<Feature>
    {
        private readonly DbHelper _db;

        public FeatureDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Feature model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Features (Name, Code)
                                VALUES (@name, @code);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@code", model.Code);
            cmd.Parameters.AddWithValue("@name", model.Name);

            model.FeatureId = Convert.ToInt32(cmd.ExecuteScalar());

            return model.FeatureId;
        }

        public bool Update(Feature model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Features
                            SET Name = @name,
                                Code = @code
                            WHERE FeatureId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@code", model.Code);
            cmd.Parameters.AddWithValue("@name", model.Name);
            cmd.Parameters.AddWithValue("@id", model.FeatureId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Features WHERE FeatureId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Feature> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Feature>();

            var cmd = new SqlCommand("SELECT * FROM Features", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Feature
                {
                    FeatureId = (int)reader["FeatureId"],
                    Name = reader["Name"].ToString(),
                    Code = reader["Code"].ToString()
                });
            }

            return list;
        }

        public List<Feature> GetFeaturesByRole(int roleId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Feature>();

            var cmd = new SqlCommand("SELECT * FROM Features WHERE FeatureId IN (SELECT FeatureId FROM RoleFeatures WHERE RoleId = @RoleId)", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@RoleId", roleId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Feature
                {
                    FeatureId = (int)reader["FeatureId"],
                    Name = reader["Name"].ToString(),
                    Code = reader["Code"].ToString()
                });
            }

            return list;
        }

        public Feature Get(int featureId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = "SELECT * FROM Features WHERE FeatureId = @FeatureId";
            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@FeatureId", featureId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Feature
                {
                    FeatureId = (int)reader["FeatureId"],
                    Name = reader["Name"].ToString(),
                    Code = reader["Code"].ToString()
                };
            }

            return null; // Not found
        }
    }
}

