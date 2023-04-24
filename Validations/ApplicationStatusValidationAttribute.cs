using System.ComponentModel.DataAnnotations;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Validations;

public class ApplicationStatusValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        //This is only needed in the updating context since the value may not be present
        if (value == null)
        {
            return true; 
        }

        if (value is not int)
        {
            ErrorMessage = "Invalid status value format";
            return false;
        }

        if (!Enum.IsDefined(typeof(JobApplicationStatus), value))
        {
            ErrorMessage = "Invalid status value";
            return false;
        }

        return true;
    }
}
