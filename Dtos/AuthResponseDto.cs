using System.Text.Json.Serialization;

namespace JobOdysseyApi.Dtos;

public class AuthResponseDto
{
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    
    [JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;
}