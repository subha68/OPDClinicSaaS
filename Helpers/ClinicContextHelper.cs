using Microsoft.AspNetCore.Http;

namespace Helpers
{
    public static class ClinicContextHelper
    {
        private const string ClinicIdKey = "ClinicId";
        private const string ClinicCodeKey = "ClinicCode";
        private const string ClinicNameKey = "ClinicName";
        private const string ClinicLogoKey = "ClinicLogo";

        public static void Save(HttpContext context, int clinicId, string clinicCode, string clinicName, string logoPath)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            };

            context.Response.Cookies.Append(ClinicIdKey, clinicId.ToString(), options);
            context.Response.Cookies.Append(ClinicCodeKey, clinicCode, options);
            context.Response.Cookies.Append(ClinicNameKey, clinicName, options);
            context.Response.Cookies.Append(ClinicLogoKey, logoPath, options);
        }

        public static int? GetClinicId(HttpContext context)
        {
            var val = context.Request.Cookies[ClinicIdKey];

            if (int.TryParse(val, out int id))
                return id;

            return null;
        }

        public static string? GetClinicCode(HttpContext context)
        {
            return context.Request.Cookies[ClinicCodeKey];
        }

        public static string? GetClinicName(HttpContext context)
        {
            return context.Request.Cookies[ClinicNameKey];
        }

        public static string? GetClinicLogo(HttpContext context)
        {
            return context.Request.Cookies[ClinicLogoKey];
        }

        public static void Clear(HttpContext context)
        {
            context.Response.Cookies.Delete(ClinicIdKey);
            context.Response.Cookies.Delete(ClinicCodeKey);
            context.Response.Cookies.Delete(ClinicNameKey);
            context.Response.Cookies.Delete(ClinicLogoKey);
        }
    }
}