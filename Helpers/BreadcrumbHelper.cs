using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class BreadcrumbHelper
{
    public static List<string> GetBreadcrumb(ViewContext context)
    {
        return new List<string>
        {
            "Home",
            context.RouteData.Values["controller"].ToString(),
            context.RouteData.Values["action"].ToString()
        };
    }
}