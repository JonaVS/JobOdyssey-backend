using System.ComponentModel.DataAnnotations;

namespace JobOdysseyApi.Dtos;

public class LoginRequestDto
{    
    [Required]
    [EmailAddress(ErrorMessage = "The Email field is not a valid")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}