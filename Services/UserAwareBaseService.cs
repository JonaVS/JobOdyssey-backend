using System.Net;
using System.Security.Claims;
using JobOdysseyApi.Core;
using JobOdysseyApi.Models;
using JobOdysseyApi.Services;

public class UserAwareBaseService : BaseService
{
    protected readonly ClaimsPrincipal? user;
    protected readonly string? userId;

    public UserAwareBaseService(
        IHttpContextAccessor httpContextAccessor, 
        CoreServiceDependencies coreServiceDependencies
        ):base(coreServiceDependencies)
    {
        user = httpContextAccessor.HttpContext?.User;
        userId = GetUserId();
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