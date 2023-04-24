using System.ComponentModel.DataAnnotations;

public class ApplicationDateValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            ErrorMessage = "Job application date field is required";
            return false;
        }

        if (!DateTime.TryParse(value.ToString(), out DateTime result))
        {
            ErrorMessage = "A valid date must be provided";
            return false;
        }

        return true;
    }
}