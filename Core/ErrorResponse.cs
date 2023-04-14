namespace JobOdysseyApi.Core;

using Microsoft.AspNetCore.Mvc.ModelBinding;

public class ErrorResponse
{
    public string Message { get; set; }
    public int ErrorCode { get; set; }
    public List<string> Errors { get; set; }

    private ErrorResponse (string message, int errorCode, List<string> errors ) 
    {
        Message = message;
        ErrorCode = errorCode;
        Errors = errors;
    }

    public static ErrorResponse Generate(string message, int errorCode, string error)
    {
        var errors = new List<string> { error };
        
        return CreateErrorObject(message, errorCode, errors);
    }

    public static ErrorResponse Generate(string message, int errorCode, ModelStateDictionary modelState)
    {
        var errors = modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        return CreateErrorObject(message, errorCode, errors);
    }

    private static ErrorResponse CreateErrorObject(string message, int errorCode, List<string> errors)
    {
        return new ErrorResponse(message, errorCode, errors);
    }
}