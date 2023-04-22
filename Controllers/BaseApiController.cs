using JobOdysseyApi.Core;
using Microsoft.AspNetCore.Mvc;

namespace JobOdysseyApi.Controllers;

public class BaseApiController : ControllerBase
{
    private const string errorType = "Action error";
    
    protected virtual ActionResult<T> HandleResult<T>(Result<T> result)
    {
        return result.Succeeded ? Ok(result.Data) 
            : StatusCode(result.ErrorCode, ErrorResponse.Generate(errorType, result.ErrorCode, result.Error!));
    }

    protected virtual ActionResult HandleResult(Result result)
    {
        return result.Succeeded ? NoContent() 
            : StatusCode(result.ErrorCode, ErrorResponse.Generate(errorType, result.ErrorCode, result.Error!));
    }
}