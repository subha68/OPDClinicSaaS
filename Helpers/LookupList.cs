using Microsoft.AspNetCore.Mvc.Rendering;

namespace Helpers
{
    public static class LookupLists
    {
        public static readonly List<SelectListItem> LookupSources = new()
        {
            new("Medicines", "Medicines")
        };
    }
}