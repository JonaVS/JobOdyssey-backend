using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class PasswordValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string password)
        {
            return false;
        }

        if (password.Length < 8)
        {
            ErrorMessage = "Password must be at least 8 characters long.";
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            ErrorMessage = "Password must contain at least one digit.";
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            ErrorMessage = "Password must contain at least one lowercase letter.";
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            ErrorMessage = "Password must contain at least one uppercase letter.";
            return false;
        }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            ErrorMessage = "Password must contain at least one symbol character.";
            return false;
        }

        return true;
    }
}