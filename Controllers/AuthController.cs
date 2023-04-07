using JobOdysseyApi.Core;
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
    public async Task<ActionResult<UserDto>> Register(RegisterDto requestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var registerResult = await _authService.Register(requestDto);

        return HandleResult<UserDto>(registerResult);
    }
}