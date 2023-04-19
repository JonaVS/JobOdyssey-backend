using System.Security.Claims;

public class UserAwareBaseService
{
    protected readonly ClaimsPrincipal? user;
    protected readonly string? userId;

    public UserAwareBaseService(IHttpContextAccessor httpContextAccessor)
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
}