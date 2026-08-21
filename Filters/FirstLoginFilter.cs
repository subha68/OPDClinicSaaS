using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Filters
{
    public class FirstLoginFilter : IActionFilter
    {
        public void OnActionExecuting(
            ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;


            /*
             * Only apply this rule to authenticated users.
             */
            if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
            {
                return;
            }


            var controller =
                context.RouteData.Values["controller"]?.ToString();

            var action =
                context.RouteData.Values["action"]?.ToString();


            /*
             * Actions that must remain accessible during
             * first login.
             */
            if (IsAllowedAction(controller, action))
            {
                return;
            }


            /*
             * Is this user's first login?
             */
            var isFirstLogin =
                httpContext.Session.GetString("IsFirstLogin");


            if (!string.Equals(
                    isFirstLogin,
                    "True",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }


            /*
             * First-login user is trying to access another
             * part of the application.
             *
             * Send them to Home/Index where the mandatory
             * Change Password modal is displayed.
             */
            context.Result = new RedirectToActionResult(
                "Index",
                "Home",
                null);
        }


        private static bool IsAllowedAction(
            string? controller,
            string? action)
        {
            if (string.IsNullOrWhiteSpace(controller))
            {
                return false;
            }


            /*
             * Home/Index is allowed because it is where
             * the mandatory Change Password modal lives.
             */
            if (string.Equals(
                    controller,
                    "Home",
                    StringComparison.OrdinalIgnoreCase)
                &&
                string.Equals(
                    action,
                    "Index",
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }


            /*
             * Account actions required during authentication
             * and password change.
             */
            if (string.Equals(
                    controller,
                    "Account",
                    StringComparison.OrdinalIgnoreCase))
            {
                return
                    string.Equals(
                        action,
                        "Login",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    string.Equals(
                        action,
                        "ChangePassword",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    string.Equals(
                        action,
                        "Logout",
                        StringComparison.OrdinalIgnoreCase);
            }


            return false;
        }

        public void OnActionExecuted(
            ActionExecutedContext context)
        {
            // Nothing required here.
        }
    }
}