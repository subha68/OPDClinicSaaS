using Microsoft.AspNetCore.Http;

namespace Helpers
{
    public class SessionHelper
    {
        private readonly IHttpContextAccessor _context;

        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor;
        }

        public int ClinicId =>
            int.Parse(_context.HttpContext.Session.GetString("ClinicId") ?? "0");

        public int RoleId =>
            int.Parse(_context.HttpContext.Session.GetString("RoleId") ?? "0");

        public int UserId =>
            int.Parse(_context.HttpContext.Session.GetString("UserId") ?? "0");

        public string RoleName =>
            _context.HttpContext.Session.GetString("RoleName") ?? "";

    }
}