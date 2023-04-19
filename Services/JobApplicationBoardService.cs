using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using JobOdysseyApi.Core;
using JobOdysseyApi.Data;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Services;

public class JobApplicationBoardService : UserAwareBaseService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobApplicationBoardService(
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        AppDbContext dbContext, IMapper mapper) : base(httpContextAccessor)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<JobBoardResponseDto>> CreateBoard(JobBoardCreateRequestDto createData)
    {
        try
        {
            if (userId is null) return Result<JobBoardResponseDto>.Failure("Invalid user ID", (int)HttpStatusCode.BadRequest);

            var dbUser = await _userManager.FindByIdAsync(userId);

            if (dbUser is null) return Result<JobBoardResponseDto>.Failure("User not found in database", (int)HttpStatusCode.BadRequest);

            var newJobBoard = new JobApplicationBoard()
            {
                Name = createData.Name,
                User = dbUser
            };

            await _dbContext.JobApplicationBoards.AddAsync(newJobBoard);
            await _dbContext.SaveChangesAsync();

            return  Result<JobBoardResponseDto>.Success(_mapper.Map<JobBoardResponseDto>(newJobBoard));
        }
        catch (Exception)
        {
            return Result<JobBoardResponseDto>.Failure("An error ocurred while creating the Job application board", (int)HttpStatusCode.InternalServerError);
        }
    }
}