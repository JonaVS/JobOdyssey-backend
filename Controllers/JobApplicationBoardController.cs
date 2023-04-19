using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Filters;
using JobOdysseyApi.Services;

namespace JobOdysseyApi.Controllers;

[ApiController]
[Route("api/job-boards")]
public class JobApplicationBoardController : BaseApiController
{
    private readonly JobApplicationBoardService _jobBoardService;

    public JobApplicationBoardController(JobApplicationBoardService jobBoardService)
    {
        _jobBoardService = jobBoardService;
    }

    [Authorize]
    [HttpPost("create")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<ActionResult<JobBoardResponseDto>> Create(JobBoardCreateRequestDto requestDto)
    {
        return HandleResult<JobBoardResponseDto>(await _jobBoardService.CreateBoard(requestDto));
    }
}