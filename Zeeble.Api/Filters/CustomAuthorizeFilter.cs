using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Zeeble.Api.Filters
{
    public class CustomAuthorizeFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new JsonResult(new { message = "You are not authorized to access this resource." })
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            }
        }
    }
}
