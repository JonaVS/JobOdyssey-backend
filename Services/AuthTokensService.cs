using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using JobOdysseyApi.Core;
using JobOdysseyApi.Data;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Services;

public class AuthTokensService
{
    private readonly string JwtSecret = DotNetEnv.Env.GetString("JWT_SECRET");
    private SecurityToken? generatedToken;
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthTokensService(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<AuthTokensDto>> GenerateAuthTokens(ApplicationUser user) {
        try
        {
            var authTokens = new AuthTokensDto()
            {
                Token = GenerateJwtToken(user),
                RefreshToken = await GenerateRefreshToken(user.Id),
                User = user
            };

            return Result<AuthTokensDto>.Success(authTokens);
        }
        catch (Exception)
        { 
           return Result<AuthTokensDto>.Failure("An error has occurred.", (int)HttpStatusCode.InternalServerError); 
        }
    }

    private string GenerateJwtToken(ApplicationUser user) 
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(JwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new [] 
            {
                new Claim("id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
            }),

            Expires = DateTime.Now.ToUniversalTime().AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        generatedToken = jwtTokenHandler.CreateToken(tokenDescriptor);

        return jwtTokenHandler.WriteToken(generatedToken);
    }

    private async Task<string> GenerateRefreshToken(string userId)
    {

        var refreshToken = new RefreshToken()
        {
            JwtId = generatedToken!.Id,
            UserId = userId,
            Token = GenerateRefreshTokenString(),
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMonths(1),
            IsRevoked = false,
            IsUsed = false
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken.Token;
    }

    public async Task<Result<AuthTokensDto>> VerifyAndRefreshTokens(string token, string refreshToken)
    {
        try
        {
            //****JWT TOKEN VALIDATIONS START****
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            if (!jwtTokenHandler.CanReadToken(token))
            {
                return Result<AuthTokensDto>.Failure("Invalid JWT token format", (int)HttpStatusCode.BadRequest);
            } 
                
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                /*
                    This needs to be false here. 
                    If set to true and the incoming JWT is expired,
                    then an exception will be trown inside the ValidateToken method and the user will never get a refreshed token
                */ 
                ValidateLifetime = false,
            };

            var tokenInVerification = jwtTokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken );

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
               var isValidAlgHeader = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase); 

               if (!isValidAlgHeader) return Result<AuthTokensDto>.Failure("Invalid JWT token", (int)HttpStatusCode.BadRequest);
            }
            //****JWT TOKEN VALIDATIONS END****
            

            //****REFRESH TOKEN VALIDATIONS START****
            var storedRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken is null 
                || storedRefreshToken.ExpiresAt < DateTime.UtcNow
                || storedRefreshToken.JwtId != validatedToken.Id 
                || storedRefreshToken.IsUsed 
                || storedRefreshToken.IsRevoked)
            {
                return Result<AuthTokensDto>.Failure("Invalid refresh token", (int)HttpStatusCode.BadRequest);
            }

            var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);

            if (user is null)
            {
                return Result<AuthTokensDto>.Failure("Invalid tokens", (int)HttpStatusCode.BadRequest);
            }
            //****REFRESH TOKEN VALIDATIONS END****

            storedRefreshToken.IsUsed = true;
            await _dbContext.SaveChangesAsync();

            return await GenerateAuthTokens(user);

        }
        catch (SecurityTokenException)
        {
          //Several errors could happen inside JwtSecurityTokenHandler.ValidateToken method.
          return Result<AuthTokensDto>.Failure("Invalid JWT token", (int)HttpStatusCode.BadRequest); 
        }
        catch (Exception)
        {
          return Result<AuthTokensDto>.Failure("An error occurred while refreshing the access token", (int)HttpStatusCode.InternalServerError); 
        }
    }

    private string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
