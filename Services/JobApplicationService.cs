using System.Net;
using JobOdysseyApi.Core;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Services;


public class JobApplicationService : UserAwareBaseService
{
    private readonly JobApplicationBoardService _jobApplicationBoardService;

    public JobApplicationService(
       IHttpContextAccessor httpContextAccessor,
       CoreServiceDependencies coreServiceDependencies,
       JobApplicationBoardService jobApplicationBoardService
    ) : base(httpContextAccessor, coreServiceDependencies)
    {
        _jobApplicationBoardService = jobApplicationBoardService;
    }

    public async Task<Result<JobApplicationDto>> CreateJobApplication(CreateJobApplicationDto createData)
    {
        try
        {
            var ownershipResult = await _jobApplicationBoardService.VerifyBoardOwnership(createData.JobBoardId);

            if (!ownershipResult.Succeeded) return Result<JobApplicationDto>.Failure(ownershipResult.Error, ownershipResult.ErrorCode);

            var jobBoard = ownershipResult.Data;

            var newJobApplication = new JobApplication()
            {
                JobTitle = createData.JobTitle,
                CompanyName = createData.CompanyName,
                ApplicationDate = new DateTimeOffset((DateTime)createData.ApplicationDate!, TimeSpan.Zero),
                JobDescription = createData.JobDescription,
                JobUrl = createData.JobUrl,
                Notes = createData.Notes,
                Status = (JobApplicationStatus)Enum.Parse(typeof(JobApplicationStatus), createData.Status.ToString()!),
                User = jobBoard!.User,
                JobBoard = jobBoard
            };

            await _dbContext.JobApplications.AddAsync(newJobApplication);
            await _dbContext.SaveChangesAsync();

            return Result<JobApplicationDto>.Success(_mapper.Map<JobApplicationDto>(newJobApplication));
        }
        catch (Exception)
        {
            return Result<JobApplicationDto>.Failure("An error ocurred while creating the job application", (int)HttpStatusCode.InternalServerError);
        }
    }
}