using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace JobOdysseyApi.Core;

public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public ApiException(string message) : base(message){}

    public static ApiException CreateException(string title, int statusCode, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = detail
        };

        return new ApiException(JsonSerializer.Serialize(problemDetails), problemDetails.Status.Value);
    }
}