using System.Net;
using AutoMapper;
using JobOdysseyApi.Core;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Models;
using Microsoft.AspNetCore.Identity;

namespace JobOdysseyApi.Services;


public class AuthService : UserAwareBaseService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuthTokensService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        IHttpContextAccessor httpContextAccessor, 
        UserManager<ApplicationUser> userManager, 
        AuthTokensService tokenService, 
        IMapper mapper) : base (httpContextAccessor, userManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<Result<AuthResponseDto>> Register(RegisterRequestDto registerData)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(registerData.Email);

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

            var dbResult = await _userManager.CreateAsync(newUser, registerData.Password);

            if (!dbResult.Succeeded)
            {
                return Result<AuthResponseDto>.Failure("An error has occurred. Please try again", (int)HttpStatusCode.InternalServerError);
            }

            var authTokensResult = await _tokenService.GenerateAuthTokens(newUser);

            if (!authTokensResult.Succeeded)
            {
                return Result<AuthResponseDto>.Failure(authTokensResult.Error, authTokensResult.ErrorCode);
            }

            return Result<AuthResponseDto>.Success(GetAuthResponseDto(authTokensResult.Data!));
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
            var user = await _userManager.FindByEmailAsync(loginData.Email);

            if (user is null)
            {
                return Result<AuthResponseDto>.Failure("Invalid credentials", (int)HttpStatusCode.BadRequest);
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginData.Password);

            if (!isValidPassword)
            {
                return Result<AuthResponseDto>.Failure("Invalid credentials", (int)HttpStatusCode.BadRequest);
            }

            var authTokensResult = await _tokenService.GenerateAuthTokens(user);

            if (!authTokensResult.Succeeded)
            {
                return Result<AuthResponseDto>.Failure(authTokensResult.Error, authTokensResult.ErrorCode);
            }

            return Result<AuthResponseDto>.Success(GetAuthResponseDto(authTokensResult.Data!));
        }
        catch (Exception)
        {
            return Result<AuthResponseDto>.Failure("An error has occurred. Please try again", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<bool>> IsAuthenticated()
    {
        if (string.IsNullOrEmpty(userId)) return Result<bool>.Failure("Invalid user ID", (int)HttpStatusCode.BadRequest);

        try
        {
            var dbUser = await _userManager.FindByIdAsync(userId);

            if (dbUser is null) return Result<bool>.Failure("User not found in database.", (int)HttpStatusCode.BadRequest);

            return Result<bool>.Success(true);

        }
        catch (Exception)
        {
            return Result<bool>.Failure("An error occurred while checking user authentication status", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<AuthResponseDto>> RefreshToken(RefreshTokenRequestDto refreshData)
    {
        var refreshResult = await _tokenService.VerifyAndRefreshTokens(refreshData.Token, refreshData.RefreshToken);

        if (!refreshResult.Succeeded)
        {
           return Result<AuthResponseDto>.Failure(refreshResult.Error, refreshResult.ErrorCode); 
        }

        return Result<AuthResponseDto>.Success(GetAuthResponseDto(refreshResult.Data!));
    }

    private AuthResponseDto GetAuthResponseDto(AuthTokensDto authTokens)
    {
        return _mapper.Map<AuthResponseDto>(authTokens.User, opt =>
        {
            opt.Items["jwtToken"] = authTokens.Token;
            opt.Items["refreshToken"] = authTokens.RefreshToken;
        });
    }
}