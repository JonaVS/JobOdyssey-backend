using JobOdysseyApi.Dtos;
using JobOdysseyApi.Services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto requestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var registerResult = await _authService.Register(requestDto);

        return HandleResult<AuthResponseDto>(registerResult);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto requestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var loginResult = await _authService.Login(requestDto);

        return HandleResult<AuthResponseDto>(loginResult);
    }
    
    private RefreshTokenRequestDto? GetTokens()
    {
        string? token;
        string? refreshToken;

        token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        refreshToken = Request.Cookies["refreshToken"];

        if (token is null || refreshToken is null) return null;
        
        return new RefreshTokenRequestDto() 
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }
}