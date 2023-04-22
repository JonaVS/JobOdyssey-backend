using System.Net;
using JobOdysseyApi.Core;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Services;


public class AuthService : UserAwareBaseService
{
    private readonly AuthTokensService _tokenService;

    public AuthService(
        IHttpContextAccessor httpContextAccessor, 
        AuthTokensService tokenService, 
        CoreServiceDependencies coreServiceDependencies
    ) : base (httpContextAccessor, coreServiceDependencies)
    {
        _tokenService = tokenService;
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
            var userResult = await CheckUserExistence();
            
            if (!userResult.Succeeded) return Result<bool>.Failure(userResult.Error, userResult.ErrorCode); 

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