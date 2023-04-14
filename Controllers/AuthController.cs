using System.Net;
using Microsoft.AspNetCore.Mvc;
using JobOdysseyApi.Core;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Filters;
using JobOdysseyApi.Services;

namespace JobOdysseyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto requestDto)
    {        
        var registerResult = await _authService.Register(requestDto);

        return HandleResult<AuthResponseDto>(registerResult);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto requestDto)
    {
        var loginResult = await _authService.Login(requestDto);

        return HandleResult<AuthResponseDto>(loginResult);
    }

    [HttpPost("refresh-token")]
    [ServiceFilter(typeof(RefreshTokenValidationFilterAttribute))]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken()
    {
        var refreshResult = await _authService.RefreshToken((RefreshTokenRequestDto)HttpContext.Items["tokens"]!);

        return HandleResult<AuthResponseDto>(refreshResult);
    }

    protected override ActionResult<T> HandleResult<T>(Result<T> result)
    {
        if (typeof(T) == typeof(AuthResponseDto))
        {
            if (result.Succeeded && result.Data is AuthResponseDto authResult)
            {
                SetRefreshTokenCookie(authResult.RefreshToken);
            }
        }

        return base.HandleResult(result);
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = DotNetEnv.Env.GetString("ASPNETCORE_ENVIRONMENT") == "Production" ? true : false,
            Expires = DateTime.UtcNow.AddMonths(1),
        });
    }
}