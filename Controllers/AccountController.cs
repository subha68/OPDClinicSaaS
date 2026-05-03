using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OPDClinic.Web.DAL;

namespace OPDClinic.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserDAL _userDAL;

        public AccountController(IConfiguration configuration)
        {
            _userDAL = new UserDAL(configuration);
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}