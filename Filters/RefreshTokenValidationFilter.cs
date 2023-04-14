using System.Net;
using JobOdysseyApi.Core;
using JobOdysseyApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JobOdysseyApi.Filters;
public class RefreshTokenValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var tokens = GetTokens(context);
        
        if (tokens == null)
        {
            context.Result = new BadRequestObjectResult(
                ErrorResponse.Generate(
                    "Validation error",
                    (int)HttpStatusCode.BadRequest,
                    "A JWT token and refresh token must be provided"
                )
             );
        }
        else
        {
            context.HttpContext.Items["tokens"] = tokens;
        }
    }
    public void OnActionExecuted(ActionExecutedContext context) {}

    private RefreshTokenRequestDto? GetTokens(ActionExecutingContext context)
    {
        string? token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        string? refreshToken = context.HttpContext.Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken)) return null;

        return new RefreshTokenRequestDto()
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }
}