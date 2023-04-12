using JobOdysseyApi.Core;
using Microsoft.AspNetCore.Mvc;

namespace JobOdysseyApi.Controllers;

public class BaseApiController : ControllerBase
{
    protected virtual ActionResult<T> HandleResult<T>(Result<T> result)
    {
        return result.Succeeded ?
            (result.Data is not null ? Ok(result.Data) : NoContent()) : StatusCode(result.ErrorCode, result);
    }
}