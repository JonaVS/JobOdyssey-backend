using System.Net;
using JobOdysseyApi.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JobOdysseyApi.Filters;
public class LogoutValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var refreshTokenCookie = context.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "refresh_token");

        if (string.IsNullOrEmpty(refreshTokenCookie.Value))
        {
           context.Result = context.Result = new BadRequestObjectResult(
                ErrorResponse.Generate(
                    "Validation error",
                    (int)HttpStatusCode.BadRequest,
                    "A refresh token must be provided"
                )
             );
        }else {
           context.HttpContext.Items["refresh_token"] = refreshTokenCookie.Value;  
        }
    }
    public void OnActionExecuted(ActionExecutedContext context) {}
}