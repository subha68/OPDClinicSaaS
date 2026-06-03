using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId"); 
            //var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId")

            if (userId == null)
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}