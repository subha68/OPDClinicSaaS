using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class FeatureDAL
    {
        private readonly DbHelper _db;

        public FeatureDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public void AddFeature(Feature feature)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Features (Name, Code)
                                 VALUES (@name, @code)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@code", feature.Code);
                cmd.Parameters.AddWithValue("@name", feature.Name);

                cmd.ExecuteNonQuery();
            }
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
    }
}

