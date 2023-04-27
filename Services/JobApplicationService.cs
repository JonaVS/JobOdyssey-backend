using System.Net;
using Microsoft.EntityFrameworkCore;
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
                Status = (JobApplicationStatus)createData.Status!,
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

    public async Task<Result<JobApplicationDto>> GetJobApplicationById(string applicationId)
    {
        try
        {
            var ownershipResult = await VerifyJobApplicationOwnership(applicationId);

            if (!ownershipResult.Succeeded) return Result<JobApplicationDto>.Failure(ownershipResult.Error, ownershipResult.ErrorCode);

            return Result<JobApplicationDto>.Success(_mapper.Map<JobApplicationDto>(ownershipResult.Data));
        }
        catch (Exception)
        {
            return Result<JobApplicationDto>.Failure("An error ocurred while fetching the specified job application", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result> UpdateJobApplicationStatus(UpdateJobApplicationStatusDto statusData, string applicationId)
    {
        try
        {
            var ownershipResult = await VerifyJobApplicationOwnership(applicationId);

            if (!ownershipResult.Succeeded) return Result.Failure(ownershipResult.Error, ownershipResult.ErrorCode);

            var targetApplication = ownershipResult.Data;

            targetApplication!.Status = (JobApplicationStatus)statusData.Status!;

            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure("An error ocurred while updating the specified job application", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result> UpdateJobApplicationContent(UpdateJobApplicationDto updateData, string applicationId)
    {
        try
        {
            var ownershipResult = await VerifyJobApplicationOwnership(applicationId);

            if (!ownershipResult.Succeeded) return Result.Failure(ownershipResult.Error, ownershipResult.ErrorCode);

            var targetApplication = ownershipResult.Data;
            var dtoProperties = typeof(UpdateJobApplicationDto).GetProperties();
            var modelProperties = typeof(JobApplication).GetProperties();

            //Update the application date first if applicable since it needs special treatment
            if (updateData.ApplicationDate is not null)
            {
                targetApplication!.ApplicationDate = new DateTimeOffset((DateTime)updateData.ApplicationDate!, TimeSpan.Zero);
                /*
                    At this point is updated, the value is set to null.
                    As null, the below foreach is going to ignore the field.
                */
                updateData.ApplicationDate = null;
            }

            //Check every field inside the DTO and update the db entity model accordingly 
            foreach (var dtoProperty in dtoProperties)
            {
                var dtoValue = dtoProperty.GetValue(updateData);

                if (dtoValue is not null)
                {
                    var modelProperty = modelProperties.FirstOrDefault(p => p.Name == dtoProperty.Name);

                    if (modelProperty is not null)
                    {
                        modelProperty.SetValue(targetApplication, dtoValue);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception)
        { 
            return Result.Failure("An error ocurred while updating the specified job application", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result> DeleteJobApplication(string applicationId)
    {
        try
        {
            var ownershipResult = await VerifyJobApplicationOwnership(applicationId);

            if (!ownershipResult.Succeeded) return Result.Failure(ownershipResult.Error, ownershipResult.ErrorCode);

            var targetApplication = ownershipResult.Data;

            _dbContext.JobApplications.Remove(targetApplication!);

            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception)
        {
           return Result.Failure("An error ocurred while deleting the specified job application", (int)HttpStatusCode.InternalServerError);
        }
    }

    private async Task<Result<JobApplication>> VerifyJobApplicationOwnership(string applicationId)
    {
       var checkUserExistenceResult = await CheckUserExistence();

       if (!checkUserExistenceResult.Succeeded)
       {
         return Result<JobApplication>.Failure(checkUserExistenceResult.Error, checkUserExistenceResult.ErrorCode);
       }

       var jobApplication = await _dbContext.JobApplications.FirstOrDefaultAsync(x => x.Id == applicationId && x.User.Id == userId);

       if (jobApplication is null) return Result<JobApplication>.Failure("The specified job application does not exist or is inaccessible.", (int)HttpStatusCode.NotFound);
    
       return Result<JobApplication>.Success(jobApplication);
    }
}