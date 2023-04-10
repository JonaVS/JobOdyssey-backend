namespace JobOdysseyApi.Dtos;

public class AuthTokensDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}