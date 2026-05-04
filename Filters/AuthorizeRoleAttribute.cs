using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Filters
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly int[] _roles;

        public AuthorizeRoleAttribute(params int[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var path = context.HttpContext.Request.Path.Value;

            // 🔥 Allow login page ALWAYS
            if (path.Contains("/Account/Login"))
                return;

            var roleId = context.HttpContext.Session.GetInt32("RoleId");

            if (roleId == null || !_roles.Contains(roleId.Value))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}