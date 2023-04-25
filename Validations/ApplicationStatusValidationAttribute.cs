using System.ComponentModel.DataAnnotations;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Validations;

/*
    Json converter runs before this.
    It will set the field as null if it cant get a valid int from the json payload
*/
public class ApplicationStatusValidationAttribute : ValidationAttribute
{
    private readonly bool _allowNull;

    public ApplicationStatusValidationAttribute(bool allowNull = false)
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
            ErrorMessage = "A valid status field is required";
            return false;
        }
        
        if (value is not null && !Enum.IsDefined(typeof(JobApplicationStatus), value))
        {
            ErrorMessage = "Invalid status value for a job application";
            return false;
        }

        return true;
    }
}
