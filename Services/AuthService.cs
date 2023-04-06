using System.Net;
using JobOdysseyApi.Core;
using JobOdysseyApi.Dtos;
using Microsoft.AspNetCore.Identity;

namespace JobOdysseyApi.Services;


public class AuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtService _tokenService;

    public AuthService(UserManager<IdentityUser> userManager, JwtService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<UserDto> Register(RegisterDto signUpData)
    {
        var user = await _userManager.FindByEmailAsync(signUpData.Email!);

        if (user is not null)
        {
           throw ApiException.CreateException("Email already in use", (int)HttpStatusCode.BadRequest, "Register error");
        }

        var newUser = new IdentityUser()
        {
            Email = signUpData.Email,
            UserName = signUpData.UserName,
        };

        var dbResult = await _userManager.CreateAsync(newUser, signUpData.Password!);

        if (!dbResult.Succeeded)
        {
            throw ApiException.CreateException("Server Error", (int)HttpStatusCode.InternalServerError, "An error ocurred during the registration process");
        }

        var jwtToken = _tokenService.GenerateJwtToken(newUser);

        return new UserDto() 
        {
            UserName = newUser.UserName!,
            Email = newUser.Email!,
            Token = jwtToken
        };
    }
}