using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DAL;
using BLL;
using Models;
using ViewModels;
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
                HttpContext.Session.SetString("IsFirstLogin", user.IsFirstLogin.ToString());

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordVM model)
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return Json(new
                {
                    success = false,
                    message = "Your session has expired. Please login again."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => new
                    {
                        field = x.Key,
                        message = string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Invalid value."
                            : e.ErrorMessage
                    }))
                    .ToList();

                return Json(new
                {
                    success = false,
                    message = "Please correct the errors below.",
                    errors
                });
            }

            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(new
                {
                    success = false,
                    message = "Unable to identify the logged-in user."
                });
            }

            if (model.CurrentPassword == model.NewPassword)
            {
                return Json(new
                {
                    success = false,
                    message = "The new password must be different from the current password.",
                    errors = new[]
                    {
                        new
                        {
                            field = "NewPassword",
                            message = "The new password must be different from the current password."
                        }
                    }
                });
            }

            var result = _user.ChangePassword(userId, model.CurrentPassword, model.NewPassword);

            if (!result.Success)
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }

            // Keep the current session in sync.
            HttpContext.Session.SetString("IsFirstLogin", "False");

            return Json(new
            {
                success = true,
                message = "Your password has been changed successfully."
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