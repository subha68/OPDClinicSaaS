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

        public int Add(MenuItem menuItem, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Menus (Title, Controller, Action, Icon)
                                VALUES (@title, @controller, @action, @icon);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@title", menuItem.Title);
            cmd.Parameters.AddWithValue("@controller", menuItem.Controller);
            cmd.Parameters.AddWithValue("@action", menuItem.Action);
            cmd.Parameters.AddWithValue("@icon", menuItem.Icon);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(MenuItem menuItem, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Menus
                                SET Title = @title,
                                    Controller = @controller,
                                    Action = @action,
                                    Icon = @icon
                                WHERE MenuId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@title", menuItem.Title);
            cmd.Parameters.AddWithValue("@controller", menuItem.Controller);
            cmd.Parameters.AddWithValue("@action", menuItem.Action);
            cmd.Parameters.AddWithValue("@icon", menuItem.Icon);
            cmd.Parameters.AddWithValue("@id", menuItem.MenuId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Menus WHERE MenuId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<MenuItem> GetAll(SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<MenuItem>();

            var cmd = new SqlCommand("SELECT * FROM Menus ORDER BY DisplayOrder", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var menu = cmd.ExecuteReader();

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

            return list;
        }

        public List<MenuItem> GetMenuItems(string featureCsv, SqlConnection conn, SqlTransaction? tran = null)
        {
            var features = featureCsv.Split(',').ToList();
            var list = new List<MenuItem>();

            var cmd = new SqlCommand("SELECT * FROM Menus ORDER BY DisplayOrder", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var menu = cmd.ExecuteReader();

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

            return list;
        }

        public MenuItem Get(int id, SqlConnection conn, SqlTransaction? tran = null)
        {
            var cmd = new SqlCommand("SELECT * FROM Menus WHERE MenuId = @id", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@id", id);
            using var menu = cmd.ExecuteReader();

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

            return null;
        }
    }
}

