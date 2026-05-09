using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class BreadcrumbHelper
{
    public static List<string> GetBreadcrumb(ViewContext context)
    {
        var controller = context?.RouteData?.Values["controller"]?.ToString() ?? "";
        var action = context?.RouteData?.Values["action"]?.ToString() ?? "";

        var crumbs = new List<string> { "Home" };

        if (!string.IsNullOrEmpty(controller))
            crumbs.Add(controller);

        if (!string.IsNullOrEmpty(action))
            crumbs.Add(action);

        return crumbs;
    }
}