using System.ComponentModel.DataAnnotations;

namespace JobOdysseyApi.Dtos;

public class RegisterRequestDto
{
    [Required]
    [MinLength(2, ErrorMessage = "DisplayName must contain at least 2 characters")]
    public string DisplayName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress(ErrorMessage = "The Email field is not a valid")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [PasswordValidation]
    public string Password { get; set; } = string.Empty;
}