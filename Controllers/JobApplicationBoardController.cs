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
    public async Task<ActionResult<JobBoardDto>> Create(CreateJobBoardDto requestDto)
    {
        return HandleResult<JobBoardDto>(await _jobBoardService.CreateBoard(requestDto));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<JobBoardDto>>> GetBoards()
    {
        return HandleResult<List<JobBoardDto>>(await _jobBoardService.GetBoards());
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<List<PopulatedJobBoardDto>>> GetBoardById(string id)
    {
        return HandleResult<List<PopulatedJobBoardDto>>(await _jobBoardService.GetBoardById(id));
    }
}