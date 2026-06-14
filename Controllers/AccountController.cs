using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DAL;
using BLL;
using Helpers;

namespace Controllers   
{
    public class AccountController : Controller
    {
        private readonly UserService _user;
        private readonly ClinicService _clinic;

        public AccountController(UserService user, ClinicService clinic)
        {
            _user = user;
            _clinic = clinic;
        }

        [HttpGet]
        public IActionResult Login()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            var clinicId = ClinicContextHelper.GetClinicId(HttpContext);
            var clinicCode = ClinicContextHelper.GetClinicCode(HttpContext);
            var clinicName = ClinicContextHelper.GetClinicName(HttpContext);
            var clinicLogo = ClinicContextHelper.GetClinicLogo(HttpContext);

            HttpContext.Session.SetString("ClinicId", clinicId == null ? "" : clinicId.Value.ToString());
            HttpContext.Session.SetString("ClinicCode", clinicCode ?? "");
            HttpContext.Session.SetString("ClinicName", clinicCode ?? "");
            HttpContext.Session.SetString("ClinicLogo", clinicCode ?? "");

            ViewBag.ClinicId = clinicId;
            ViewBag.ClinicCode = clinicCode;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var clinicId = ClinicContextHelper.GetClinicId(HttpContext);
            var clinicCode = ClinicContextHelper.GetClinicCode(HttpContext);
            var clinicName = ClinicContextHelper.GetClinicName(HttpContext);
            var clinicLogo = ClinicContextHelper.GetClinicLogo(HttpContext);

            HttpContext.Session.SetString("ClinicId", clinicId == null ? "" : clinicId.Value.ToString());
            HttpContext.Session.SetString("ClinicCode", clinicCode ?? "");
            HttpContext.Session.SetString("ClinicName", clinicName);
            HttpContext.Session.SetString("ClinicLogo", clinicLogo);

            ViewBag.ClinicCode = clinicCode;

            var user = _user.ValidateUser(username, password);

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

                var principal = new System.Security.Claims.ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // keep your session too
                String rolename = _user.GetUserRole(user).RoleName;
                int doctorId = _user.GetUserDoctor(user)?.DoctorId ?? 0;

                if (doctorId != 0)
                {
                    HttpContext.Session.SetString("DoctorId", doctorId.ToString());
                }
                HttpContext.Session.SetString("RoleId", user.RoleId.ToString());
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetString("RoleName", rolename);
                HttpContext.Session.SetString("FirstName", user.FirstName ?? "");
                HttpContext.Session.SetString("LastName", user.LastName ?? "");
                HttpContext.Session.SetString("DisplayName", (user.FirstName ?? "") + " " + (user.LastName ?? ""));
                HttpContext.Session.SetString("Features", _user.GetUserFeaturesAsString(user));

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

        [HttpPost]
        public IActionResult ValidateOrganisation(string clinicCode)
        {
            if (string.IsNullOrWhiteSpace(clinicCode))
            {
                return Json(new
                {
                    success = false,
                    message = "Organisation code required."
                });
            }

            var clinic = _clinic.ValidateClinicCode(clinicCode);

            if (clinic == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid organisation code."
                });
            }

            HttpContext.Session.SetString("ClinicCode", clinic.ClinicCode);
            HttpContext.Session.SetString("ClinicId", clinic.ClinicId.ToString());
            HttpContext.Session.SetString("ClinicName", clinic.Name.ToString());
            HttpContext.Session.SetString("ClinicLogo", clinic.LogoPath.ToString());

            ClinicContextHelper.Save(HttpContext, clinic.ClinicId, clinic.ClinicCode, clinic.Name, clinic.LogoPath);

            return Json(new
            {
                success = true,
                clinicCode = clinic.ClinicCode,
                clinicId = clinic.ClinicId
            });
        }       

        [HttpPost]
        public IActionResult ClearOrganisation()
        {
            HttpContext.Session.Remove("ClinicCode");
            HttpContext.Session.Remove("ClinicId");
            HttpContext.Session.Remove("ClinicName");
            HttpContext.Session.Remove("ClinicLogo");

            ClinicContextHelper.Clear(HttpContext);

            return Json(new
            {
                success = true
            });
        }

        // use - https://localhost:7036/Account/SuperAdminSetup
        public IActionResult SuperAdminSetup()
        {
            _user.SetupSuperAdmin();
            return RedirectToAction("Login", "Account");
        }

    }
}