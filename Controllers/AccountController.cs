using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DAL;
using BLL;

namespace Controllers   
{
    public class AccountController : Controller
    {
        private readonly UserDAL _userDAL;
        private readonly SessionHelper _session;
        private readonly FeatureService _featureService;

        public AccountController(IConfiguration configuration, SessionHelper session)
        {
            _userDAL = new UserDAL(configuration);
            _session = session;
            _featureService = new FeatureService(configuration);
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

            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var err in errors)
                {
                    Console.WriteLine($"Field: {key} → Error: {err.ErrorMessage}");
                }
            }

            if (user != null)
            {
                Console.WriteLine($"User validated: User Id - {user.UserId}, Role Id - {user.RoleId}, Clinic Id - {user.ClinicId}");
                HttpContext.Session.SetString("RoleId", user.RoleId.ToString());
                HttpContext.Session.SetString("UserId", user.UserId.ToString());

                if (user.ClinicId.HasValue)
                {
                    HttpContext.Session.SetString("ClinicId", user.ClinicId?.ToString()?? "0");
                }
                var features = _featureService.GetFeatures(user.RoleId);

                // store as CSV
                HttpContext.Session.SetString("Features", string.Join(",", features.Select(f => f.Code)));
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

    }
}