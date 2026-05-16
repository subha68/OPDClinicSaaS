using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DAL;
using BLL;

namespace Controllers   
{
    public class AccountController : Controller
    {
        private readonly UserDAL _userDAL;
        private readonly SessionHelper _session;
        private readonly FeatureService _featureService;
        private readonly RoleService _role;
        private readonly DoctorService _doctor;

        public AccountController(IConfiguration configuration, SessionHelper session)
        {
            _userDAL = new UserDAL(configuration);
            _session = session;
            _featureService = new FeatureService(configuration);
            _role = new RoleService(configuration);
            _doctor = new DoctorService(configuration);
        }

        [HttpGet]
        public IActionResult Login()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
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
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim("UserId", user.UserId.ToString()),
                    new System.Security.Claims.Claim("RoleId", user.RoleId.ToString())
                };

                if (user.ClinicId.HasValue)
                {
                    claims.Add(
                        new System.Security.Claims.Claim(
                            "ClinicId",
                            user.ClinicId.Value.ToString()));
                }

                var identity = new System.Security.Claims.ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal =
                    new System.Security.Claims.ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // keep your session too
                String rolename = _role.GetRoleName(user.RoleId);
                if (rolename == "Doctor")
                {
                    int doctorid = _doctor.GetDoctorIdByUserId(user.UserId);
                    HttpContext.Session.SetString("DoctorId", doctorid.ToString());
                }
                HttpContext.Session.SetString("RoleId", user.RoleId.ToString());
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetString("RoleName", rolename);
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);

                if (user.ClinicId.HasValue)
                    HttpContext.Session.SetString("ClinicId", user.ClinicId.Value.ToString());

                var features = _featureService.GetFeatures(user.RoleId);

                HttpContext.Session.SetString(
                    "Features",
                    string.Join(",", features.Select(f => f.Code)));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Remove auth cookie
            return RedirectToAction("Login", "Account");
        }        

    }
}