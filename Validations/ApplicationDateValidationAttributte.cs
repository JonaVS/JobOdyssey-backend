using System.ComponentModel.DataAnnotations;

/*
    Json converter runs before this.
    It will set the field as null if it cant get a valid date from the json payload
*/
public class ApplicationDateValidationAttribute : ValidationAttribute
{
    private readonly bool _allowNull;

    public ApplicationDateValidationAttribute(bool allowNull = false)
    {
        _allowNull = allowNull;
    }
    public override bool IsValid(object? value)
    {
        if (_allowNull && value is null)
        {
            return true;
        }
        else if (!_allowNull && value is null)
        {
            ErrorMessage = "A valid applicationDate field is required";
            return false;
        }

        return true;
    }
}