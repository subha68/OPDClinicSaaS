using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DAL;

namespace Controllers
{
    public class AccountController : Controller
    {
        private readonly UserDAL _userDAL;

        public AccountController(IConfiguration configuration)
        {
            _userDAL = new UserDAL(configuration);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _userDAL.ValidateUser(username, password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetInt32("RoleId", user.RoleId);

                if (user.ClinicId.HasValue)
                    HttpContext.Session.SetInt32("ClinicId", user.ClinicId.Value);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

    }
}