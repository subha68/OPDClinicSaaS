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

        public int Add(Feature model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Features (Name, Code)
                                 VALUES (@name, @code);
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@code", model.Code);
                cmd.Parameters.AddWithValue("@name", model.Name);

                model.FeatureId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return model.FeatureId;
        }

        public bool Update(Feature model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"UPDATE Features
                                SET Name = @name,
                                    Code = @code
                                WHERE FeatureId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@code", model.Code);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@id", model.FeatureId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"DELETE FROM Features WHERE FeatureId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<Feature> GetAll()
        {
            var list = new List<Feature>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Features", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Feature
                    {
                        FeatureId = (int)reader["FeatureId"],
                        Name = reader["Name"].ToString(),
                        Code = reader["Code"].ToString()
                    });
                }
            }

            return list;
        }

        public List<Feature> GetFeaturesByRole(int roleId)
        {
            var list = new List<Feature>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Features WHERE FeatureId IN (SELECT FeatureId FROM RoleFeatures WHERE RoleId = @RoleId)", conn);
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Feature
                    {
                        FeatureId = (int)reader["FeatureId"],
                        Name = reader["Name"].ToString(),
                        Code = reader["Code"].ToString()
                    });
                }
            }

            return list;
        }

        public Feature Get(int featureId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT * FROM Features WHERE FeatureId = @FeatureId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FeatureId", featureId);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Feature
                    {
                        FeatureId = (int)reader["FeatureId"],
                        Name = reader["Name"].ToString(),
                        Code = reader["Code"].ToString()
                    };
                }
            }
            return null; // Not found
        }
    }
}

