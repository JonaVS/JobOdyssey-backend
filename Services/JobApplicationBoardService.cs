using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        AppDbContext dbContext, IMapper mapper) : base(httpContextAccessor, userManager)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<JobBoardResponseDto>> CreateBoard(JobBoardCreateRequestDto createData)
    {
        try
        {
            var userResult = await CheckUserExistence();

            if (!userResult.Succeeded) return Result<JobBoardResponseDto>.Failure(userResult.Error, userResult.ErrorCode); 

            var newJobBoard = new JobApplicationBoard()
            {
                Name = createData.Name,
                User = userResult.Data!
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

    public async Task<Result<List<JobBoardResponseDto>>> GetBoards()
    {
        try
        {
            var userResult = await CheckUserExistence();

            if (!userResult.Succeeded) return Result<List<JobBoardResponseDto>>.Failure(userResult.Error, userResult.ErrorCode);

            var boards = await _dbContext.JobApplicationBoards
                .Where(board => board.User.Id == userId)
                .ProjectTo<JobBoardResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<List<JobBoardResponseDto>>.Success(boards);
        }
        catch (Exception)
        {
            return Result<List<JobBoardResponseDto>>.Failure("An error ocurred while fetching the boards", (int)HttpStatusCode.InternalServerError);
        }
    }
}