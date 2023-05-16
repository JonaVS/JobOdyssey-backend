using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Filters;
using JobOdysseyApi.Services;

namespace JobOdysseyApi.Controllers;

[ApiController]
[Route("api/job-boards")]
[Authorize]
public class JobApplicationBoardController : BaseApiController
{
    private readonly JobApplicationBoardService _jobBoardService;

    public JobApplicationBoardController(JobApplicationBoardService jobBoardService)
    {
        _jobBoardService = jobBoardService;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<ActionResult<JobBoardDto>> Create(CreateJobBoardDto requestDto)
    {
        return HandleResult<JobBoardDto>(await _jobBoardService.CreateBoard(requestDto));
    }

    [HttpGet]
    public async Task<ActionResult<List<JobBoardDto>>> GetBoards()
    {
        return HandleResult<List<JobBoardDto>>(await _jobBoardService.GetBoards());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PopulatedJobBoardDto>> GetBoardById(string id)
    {
        return HandleResult<PopulatedJobBoardDto>(await _jobBoardService.GetBoardById(id));
    }

    [HttpPatch("{id}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<ActionResult<JobBoardDto>> UpdateBoard(string id, [FromBody] UpdateJobBoardDto requestDto)
    {
        return HandleResult<JobBoardDto>(await _jobBoardService.UpdateBoard(id, requestDto));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBoard(string id)
    {
        return HandleResult(await _jobBoardService.DeleteBoard(id));
    }
}