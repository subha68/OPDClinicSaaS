using Microsoft.AspNetCore.Mvc;
using BLL;

public class NavMenuViewComponent : ViewComponent
{
    private readonly MenuService _menuService;
    private readonly IHttpContextAccessor _context;

    public NavMenuViewComponent(MenuService menuService, IHttpContextAccessor context)
    {
        _menuService = menuService;
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var httpContext = _context.HttpContext;
        var features = httpContext.Session.GetString("Features");
        var menu = _menuService.GetMenu(features ?? "");
        var currentController = httpContext.Request.RouteValues["controller"]?.ToString();
        var currentAction = httpContext.Request.RouteValues["action"]?.ToString();
        var currentReportCode = httpContext.Request.RouteValues["reportCode"]?.ToString();

        foreach (var item in menu)
        {
            item.SetActiveState(currentController, currentAction, currentReportCode);
        }

        return View(menu);
    }
}