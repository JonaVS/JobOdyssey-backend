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

    public async Task<Result<AuthResponseDto>> Register(RegisterRequestDto registerData)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(registerData.Email!);

            if (user is not null)
            {
                return Result<AuthResponseDto>.Failure("Email already in use", (int)HttpStatusCode.BadRequest);
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
                return Result<AuthResponseDto>.Failure("An error has occurred. Please try again", (int)HttpStatusCode.InternalServerError);
            }

            var jwtToken = _tokenService.GenerateJwtToken(newUser);

            return Result<AuthResponseDto>.Success(new AuthResponseDto()
            {
                DisplayName = newUser.DisplayName!,
                Email = newUser.Email!,
                Token = jwtToken
            });
        }
        catch (Exception)
        {
            return Result<AuthResponseDto>.Failure("An error has occurred. Please try again", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<AuthResponseDto>> Login(LoginRequestDto loginData)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginData.Email!);

            if (user is null)
            {
                return Result<AuthResponseDto>.Failure("Invalid credentials", (int)HttpStatusCode.BadRequest);
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginData.Password!);

            if (!isValidPassword)
            {
                return Result<AuthResponseDto>.Failure("Invalid credentials", (int)HttpStatusCode.BadRequest);
            }

            var jwtToken = _tokenService.GenerateJwtToken(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto()
            {
                DisplayName = user.DisplayName!,
                Email = user.Email!,
                Token = jwtToken
            });
        }
        catch (Exception)
        {
            return Result<AuthResponseDto>.Failure("An error has occurred. Please try again", (int)HttpStatusCode.BadRequest);
        }
    }
}