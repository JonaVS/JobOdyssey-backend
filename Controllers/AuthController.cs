using JobOdysseyApi.Core;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobOdysseyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userDto = await _authService.Register(requestDto);
            return Ok(userDto);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }
}