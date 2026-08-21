using DAL;
using Models;

namespace BLL
{
    public class MenuService : BaseService<MenuItem>
    {
        private readonly MenuDAL _menuDAL;

        public MenuService(
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor)
            : base(config, httpContextAccessor)
        {
            _menuDAL = new MenuDAL(config);
            _dal = _menuDAL;
        }

        public List<MenuItem> GetMenu(string featureCsv)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            // Get all active menus from DB.
            // Permission filtering is handled here rather than in the DAL
            // because we need the complete hierarchy to identify parents.
            var allMenus = _menuDAL.GetAllActive(conn);

            // Convert user's feature CSV into a case-insensitive HashSet.
            var features = (featureCsv ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Lookup makes finding parents very quick.
            var menuLookup = allMenus.ToDictionary(x => x.MenuId);

            // These are the actual menu items the user is permitted to access.
            var permittedIds = allMenus
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.FeatureCode) &&
                    features.Contains(x.FeatureCode))
                .Select(x => x.MenuId)
                .ToHashSet();

            // Keep permitted items and all of their ancestors.
            var idsToKeep = new HashSet<int>(permittedIds);

            foreach (var menuId in permittedIds)
            {
                var current = menuLookup[menuId];

                while (current.ParentId.HasValue)
                {
                    var parentId = current.ParentId.Value;

                    // Parent is already included.
                    if (!idsToKeep.Add(parentId))
                        break;

                    // Safety check in case the DB contains an invalid ParentId.
                    if (!menuLookup.TryGetValue(parentId, out var parent))
                        break;

                    current = parent;
                }
            }

            // Only retain permitted menu items and their required parents.
            var filteredMenus = allMenus
                .Where(x => idsToKeep.Contains(x.MenuId))
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            // Clear Children in case MenuItem instances are reused.
            foreach (var menu in filteredMenus)
            {
                menu.Children.Clear();
            }

            // Build the hierarchy.
            var filteredLookup = filteredMenus.ToDictionary(x => x.MenuId);

            foreach (var menu in filteredMenus)
            {
                if (menu.ParentId.HasValue &&
                    filteredLookup.TryGetValue(
                        menu.ParentId.Value,
                        out var parent))
                {
                    parent.Children.Add(menu);
                }
            }

            // Make sure children respect DisplayOrder.
            foreach (var menu in filteredMenus)
            {
                menu.Children = menu.Children
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();
            }

            // The View should receive only root-level menu items.
            return filteredMenus
                .Where(x => !x.ParentId.HasValue)
                .OrderBy(x => x.DisplayOrder)
                .ToList();
        }
    }
}