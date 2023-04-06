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

    public async Task<Result<UserDto>> Register(RegisterDto registerData)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(registerData.Email!);

            if (user is not null)
            {
                return Result<UserDto>.Failure("Email already in use", (int)HttpStatusCode.BadRequest);
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
                return Result<UserDto>.Failure("An error ocurred during the registration process", (int)HttpStatusCode.InternalServerError);
            }

            var jwtToken = _tokenService.GenerateJwtToken(newUser);

            return Result<UserDto>.Success(new UserDto()
            {
                DisplayName = newUser.DisplayName!,
                Email = newUser.Email!,
                Token = jwtToken
            });
        }
        catch (Exception)
        {
            return Result<UserDto>.Failure("An error ocurred during the registration process", (int)HttpStatusCode.InternalServerError);
        }
    }
}