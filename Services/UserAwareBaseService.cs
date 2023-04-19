using System.Net;
using System.Security.Claims;
using JobOdysseyApi.Core;
using JobOdysseyApi.Models;
using Microsoft.AspNetCore.Identity;

public class UserAwareBaseService
{
    protected readonly ClaimsPrincipal? user;
    protected readonly string? userId;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserAwareBaseService(
        IHttpContextAccessor httpContextAccessor, 
        UserManager<ApplicationUser> userManager)
    {
        user = httpContextAccessor.HttpContext?.User;
        userId = GetUserId();
        _userManager = userManager;
    }

    private string? GetUserId()
    {
        if (user is null) return null;

        var userId = user.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

        if (string.IsNullOrEmpty(userId)) return null;

        return userId;
    }

    protected async Task<Result<ApplicationUser>> CheckUserExistence()
    {
        if (string.IsNullOrEmpty(userId)) return Result<ApplicationUser>.Failure("Invalid user ID", (int)HttpStatusCode.BadRequest);

        var dbUser = await _userManager.FindByIdAsync(userId);

        if (dbUser is null) return Result<ApplicationUser>.Failure("User not found in database", (int)HttpStatusCode.BadRequest);

        return Result<ApplicationUser>.Success(dbUser);
    }
}