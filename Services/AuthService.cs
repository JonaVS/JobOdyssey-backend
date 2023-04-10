using System.Net;
using AutoMapper;
using JobOdysseyApi.Core;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Models;
using Microsoft.AspNetCore.Identity;

namespace JobOdysseyApi.Services;


public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuthTokensService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(UserManager<ApplicationUser> userManager, AuthTokensService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
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

            var authTokensResult = await _tokenService.GenerateAuthTokens(newUser);

            if (!authTokensResult.Succeeded)
            {
                return Result<AuthResponseDto>.Failure(authTokensResult.Error!, authTokensResult.ErrorCode);
            }

            return Result<AuthResponseDto>.Success(GetAuthResponseDto(newUser, authTokensResult.Data!));
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

            var authTokensResult = await _tokenService.GenerateAuthTokens(user);

            if (!authTokensResult.Succeeded)
            {
                return Result<AuthResponseDto>.Failure(authTokensResult.Error!, authTokensResult.ErrorCode);
            }

            return Result<AuthResponseDto>.Success(GetAuthResponseDto(user, authTokensResult.Data!));
        }
        catch (Exception)
        {
            return Result<AuthResponseDto>.Failure("An error has occurred. Please try again", (int)HttpStatusCode.BadRequest);
        }
    }

    private AuthResponseDto GetAuthResponseDto(ApplicationUser user, AuthTokensDto authTokens)
    {
        return _mapper.Map<AuthResponseDto>(user, opt =>
        {
            opt.Items["jwtToken"] = authTokens.Token;
            opt.Items["refreshToken"] = authTokens.RefreshToken;
        });
    }
}