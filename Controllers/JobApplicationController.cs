using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Filters;
using JobOdysseyApi.Services;

namespace JobOdysseyApi.Controllers;

[ApiController]
[Route("api/job-applications")]
[Authorize]
public class JobApplicationController : BaseApiController
{
    private readonly JobApplicationService _jobApplicationService;

    public JobApplicationController(JobApplicationService jobApplicationService)
    {
        _jobApplicationService = jobApplicationService;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<ActionResult<JobApplicationDto>> CreateJobApplication(CreateJobApplicationDto requestDto)
    {
        return HandleResult<JobApplicationDto>(await _jobApplicationService.CreateJobApplication(requestDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplicationDto>> GetJobApplicationById(string id)
    {
        return HandleResult<JobApplicationDto>(await _jobApplicationService.GetJobApplicationById(id));
    }
}