using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class MenuDAL : BaseDAL, IDAL<MenuItem>
    {
        private readonly DbHelper _db;

        public MenuDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(MenuItem menuItem, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Menus (Title, Controller, Action, Icon, FeatureCode, ReportCode, ParentId, DisplayOrder, IsActive)
                                VALUES (@title, @controller, @action, @icon, @FeatureCode, @ReportCode, @ParentId, @DisplayOrder, @IsActive);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@title", menuItem.Title);
            cmd.Parameters.AddWithValue("@controller", (object?)menuItem.Controller ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@action", (object?)menuItem.Action ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@icon", (object?)menuItem.Icon ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FeatureCode", (object?)menuItem.FeatureCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReportCode", (object?)menuItem.ReportCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ParentId", (object?)menuItem.ParentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DisplayOrder", menuItem.DisplayOrder);
            cmd.Parameters.AddWithValue("@IsActive", menuItem.IsActive ? 1 : 0);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(MenuItem menuItem, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Menus
                                SET Title = @title,
                                    Controller = @controller,
                                    Action = @action,
                                    Icon = @icon,
                                    FeatureCode = @FeatureCode,
                                    ReportCode = @ReportCode,
                                    ParentId = @ParentId, 
                                    DisplayOrder = @DisplayOrder, 
                                    IsActive = @IsActive
                                WHERE MenuId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@title", menuItem.Title);
            cmd.Parameters.AddWithValue("@controller", (object?)menuItem.Controller ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@action", (object?)menuItem.Action ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@icon", (object?)menuItem.Icon ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FeatureCode", (object?)menuItem.FeatureCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReportCode", (object?)menuItem.ReportCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ParentId", (object?)menuItem.ParentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DisplayOrder", menuItem.DisplayOrder);
            cmd.Parameters.AddWithValue("@IsActive", menuItem.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", menuItem.MenuId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Menus WHERE MenuId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<MenuItem> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
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
                    Controller = menu["Controller"] != DBNull.Value ? menu["Controller"].ToString() : null,
                    Action = menu["Action"] != DBNull.Value ? menu["Action"].ToString() : null,
                    Icon = menu["Icon"] != DBNull.Value ? menu["Icon"].ToString() : null,
                    FeatureCode = menu["FeatureCode"] != DBNull.Value ? menu["FeatureCode"].ToString() : null,
                    ReportCode = menu["ReportCode"] != DBNull.Value ? menu["ReportCode"].ToString() : null,
                    ParentId = menu["ParentId"] != DBNull.Value ? Convert.ToInt32(menu["ParentId"]) : null, 
                    DisplayOrder = Convert.ToInt32(menu["DisplayOrder"]), 
                    IsActive = Convert.ToBoolean(menu["IsActive"])
                });
            }

            return list;
        }

        public List<MenuItem> GetMenuItems(string featureCsv, SqlConnection conn, SqlTransaction? tran = null)
        {
            var features = featureCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

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
                        MenuId = Convert.ToInt32(menu["MenuId"]),
                        Title = menu["Title"].ToString(),
                        Controller = menu["Controller"] != DBNull.Value ? menu["Controller"].ToString() : null,
                        Action = menu["Action"] != DBNull.Value ? menu["Action"].ToString() : null,
                        Icon = menu["Icon"] != DBNull.Value ? menu["Icon"].ToString() : null,
                        FeatureCode = menu["FeatureCode"] != DBNull.Value ? menu["FeatureCode"].ToString() : null,
                        ReportCode = menu["ReportCode"] != DBNull.Value ? menu["ReportCode"].ToString() : null,
                        ParentId = menu["ParentId"] != DBNull.Value ? Convert.ToInt32(menu["ParentId"]) : null, 
                        DisplayOrder = Convert.ToInt32(menu["DisplayOrder"]), 
                        IsActive = Convert.ToBoolean(menu["IsActive"])
                    });
                }
            }

            return list;
        }

        public MenuItem Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
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
                    Controller = menu["Controller"] != DBNull.Value ? menu["Controller"].ToString() : null,
                    Action = menu["Action"] != DBNull.Value ? menu["Action"].ToString() : null,
                    Icon = menu["Icon"] != DBNull.Value ? menu["Icon"].ToString() : null,
                    FeatureCode = menu["FeatureCode"] != DBNull.Value ? menu["FeatureCode"].ToString() : null,
                    ReportCode = menu["ReportCode"] != DBNull.Value ? menu["ReportCode"].ToString() : null,
                    ParentId = menu["ParentId"] != DBNull.Value ? Convert.ToInt32(menu["ParentId"]) : null, 
                    DisplayOrder = Convert.ToInt32(menu["DisplayOrder"]), 
                    IsActive = Convert.ToBoolean(menu["IsActive"])
                };
            }

            return null;
        }

        public List<MenuItem> GetAllActive(SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<MenuItem>();

            var cmd = new SqlCommand(@"
                SELECT MenuId, Title, Controller, Action, Icon, FeatureCode, ReportCode, ParentId, DisplayOrder, IsActive
                FROM Menus
                WHERE IsActive = 1
                ORDER BY DisplayOrder", conn);

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
                    Controller = menu["Controller"] != DBNull.Value ? menu["Controller"].ToString() : null,
                    Action = menu["Action"] != DBNull.Value ? menu["Action"].ToString() : null,
                    Icon = menu["Icon"] != DBNull.Value ? menu["Icon"].ToString() : null,
                    FeatureCode = menu["FeatureCode"] != DBNull.Value ? menu["FeatureCode"].ToString() : null,
                    ReportCode = menu["ReportCode"] != DBNull.Value ? menu["ReportCode"].ToString() : null,
                    ParentId = menu["ParentId"] != DBNull.Value ? Convert.ToInt32(menu["ParentId"]) : null,
                    DisplayOrder = Convert.ToInt32(menu["DisplayOrder"]),
                    IsActive = Convert.ToBoolean(menu["IsActive"])
                });
            }

            return list;
        }
    }
}

