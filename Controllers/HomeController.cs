using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace OPDClinic.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}