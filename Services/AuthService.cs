using System.Net;
using JobOdysseyApi.Core;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Models;
using Microsoft.AspNetCore.Identity;

namespace JobOdysseyApi.Services;


public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager, JwtService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<UserDto> Register(RegisterDto registerData)
    {
        var user = await _userManager.FindByEmailAsync(registerData.Email!);

        if (user is not null)
        {
           throw ApiException.CreateException("Email already in use", (int)HttpStatusCode.BadRequest, "Register error");
        }

        var newUser = new ApplicationUser()
        {
            Email = registerData.Email,
            UserName = registerData.Email,
            DisplayName = registerData.DisplayName
        };

        var dbResult = await _userManager.CreateAsync(newUser, registerData.Password!);

        if (!dbResult.Succeeded)
        {
            throw ApiException.CreateException("Server Error", (int)HttpStatusCode.InternalServerError, "An error ocurred during the registration process");
        }

        var jwtToken = _tokenService.GenerateJwtToken(newUser);

        return new UserDto() 
        {
            DisplayName = newUser.DisplayName!,
            Email = newUser.Email!,
            Token = jwtToken
        };
    }
}