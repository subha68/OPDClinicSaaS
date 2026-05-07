using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Filters
{
    public class AuthorizeFeatureAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _feature;

        public AuthorizeFeatureAttribute(string feature)
        {
            _feature = feature;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var features = context.HttpContext.Session.GetString("Features");

            if (string.IsNullOrEmpty(features) || !features.Contains(_feature))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}