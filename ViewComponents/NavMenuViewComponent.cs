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
        var features = _context.HttpContext.Session.GetString("Features");
        var roleId = _context.HttpContext.Session.GetString("RoleId");
        Console.WriteLine($"Features for Role Id - {roleId} = {features}");

        var menu = _menuService.GetMenu(features ?? "");

        return View(menu);
    }
}