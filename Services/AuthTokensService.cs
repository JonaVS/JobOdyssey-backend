using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
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
    public AuthTokensService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<AuthTokensDto>> GenerateAuthTokens(ApplicationUser user) {
        try
        {
            var authTokens = new AuthTokensDto()
            {
                Token = GenerateJwtToken(user),
                RefreshToken = await GenerateRefreshToken(user.Id)
            };

            return Result<AuthTokensDto>.Success(authTokens);
        }
        catch (Exception)
        { 
           return Result<AuthTokensDto>.Failure("An error has occurred.", (int)HttpStatusCode.InternalServerError); 
        }
    }

    public string GenerateJwtToken(ApplicationUser user) 
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

        await _dbContext.RefreshTokens!.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken.Token;
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
