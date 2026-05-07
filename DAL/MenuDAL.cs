using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class MenuDAL
    {
        private readonly DbHelper _db;

        public MenuDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public void AddMenuItem(MenuItem menuItem)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Menus (Title, Controller, Action, Icon)
                                 VALUES (@title, @controller, @action, @icon)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@title", menuItem.Title);
                cmd.Parameters.AddWithValue("@controller", menuItem.Controller);
                cmd.Parameters.AddWithValue("@action", menuItem.Action);
                cmd.Parameters.AddWithValue("@icon", menuItem.Icon);

                cmd.ExecuteNonQuery();
            }
        }

        public List<MenuItem> GetMenuItems(string featureCsv)
        {
            var features = featureCsv.Split(',').ToList();
            var list = new List<MenuItem>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Menus ORDER BY DisplayOrder", conn);
                var menu = cmd.ExecuteReader();

                while (menu.Read())
                {
                    string feature = menu["FeatureCode"].ToString();

                    if (features.Contains(feature))
                    {
                        list.Add(new MenuItem
                        {
                            Title = menu["Title"].ToString(),
                            Controller = menu["Controller"].ToString(),
                            Action = menu["Action"].ToString(),
                            Icon = menu["Icon"].ToString()
                        });
                    }
                }
            }

            return list;
        }
    }
}

