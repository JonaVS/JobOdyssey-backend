using System.Net;
using JobOdysseyApi.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JobOdysseyApi.Filters;
public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(ErrorResponse.Generate("Validation Error", (int)HttpStatusCode.BadRequest, context.ModelState));
        }
    }
    public void OnActionExecuted(ActionExecutedContext context) {}
}