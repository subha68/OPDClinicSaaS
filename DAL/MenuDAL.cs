using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class MenuDAL : IDAL<MenuItem>
    {
        private readonly DbHelper _db;

        public MenuDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(MenuItem menuItem)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Menus (Title, Controller, Action, Icon)
                                 VALUES (@title, @controller, @action, @icon);
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@title", menuItem.Title);
                cmd.Parameters.AddWithValue("@controller", menuItem.Controller);
                cmd.Parameters.AddWithValue("@action", menuItem.Action);
                cmd.Parameters.AddWithValue("@icon", menuItem.Icon);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Update(MenuItem menuItem)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"UPDATE Menus
                                 SET Title = @title,
                                     Controller = @controller,
                                     Action = @action,
                                     Icon = @icon
                                 WHERE MenuId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@title", menuItem.Title);
                cmd.Parameters.AddWithValue("@controller", menuItem.Controller);
                cmd.Parameters.AddWithValue("@action", menuItem.Action);
                cmd.Parameters.AddWithValue("@icon", menuItem.Icon);
                cmd.Parameters.AddWithValue("@id", menuItem.MenuId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string query = @"DELETE FROM Menus WHERE MenuId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<MenuItem> GetAll()
        {
            var list = new List<MenuItem>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Menus ORDER BY DisplayOrder", conn);
                var menu = cmd.ExecuteReader();

                while (menu.Read())
                {
                    list.Add(new MenuItem
                    {
                        MenuId = Convert.ToInt32(menu["MenuId"]),
                        Title = menu["Title"].ToString(),
                        Controller = menu["Controller"].ToString(),
                        Action = menu["Action"].ToString(),
                        Icon = menu["Icon"].ToString()
                    });
                }
            }

            return list;
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

        public MenuItem Get(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT * FROM Menus WHERE MenuId = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var menu = cmd.ExecuteReader();

                if (menu.Read())
                {
                    return new MenuItem
                    {
                        MenuId = Convert.ToInt32(menu["MenuId"]),
                        Title = menu["Title"].ToString(),
                        Controller = menu["Controller"].ToString(),
                        Action = menu["Action"].ToString(),
                        Icon = menu["Icon"].ToString()
                    };
                }
            }

            return null;
        }
    }
}

