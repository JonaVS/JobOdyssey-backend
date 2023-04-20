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

    public async Task<Result<JobBoardDto>> CreateBoard(CreateJobBoardDto createData)
    {
        try
        {
            var userResult = await CheckUserExistence();

            if (!userResult.Succeeded) return Result<JobBoardDto>.Failure(userResult.Error, userResult.ErrorCode); 

            var newJobBoard = new JobApplicationBoard()
            {
                Name = createData.Name,
                User = userResult.Data!
            };

            await _dbContext.JobApplicationBoards.AddAsync(newJobBoard);
            await _dbContext.SaveChangesAsync();

            return  Result<JobBoardDto>.Success(_mapper.Map<JobBoardDto>(newJobBoard));
        }
        catch (Exception)
        {
            return Result<JobBoardDto>.Failure("An error ocurred while creating the Job application board", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<List<JobBoardDto>>> GetBoards()
    {
        try
        {
            var userResult = await CheckUserExistence();

            if (!userResult.Succeeded) return Result<List<JobBoardDto>>.Failure(userResult.Error, userResult.ErrorCode);

            var boards = await _dbContext.JobApplicationBoards
                .Where(board => board.User.Id == userId)
                .ProjectTo<JobBoardDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<List<JobBoardDto>>.Success(boards);
        }
        catch (Exception)
        {
            return Result<List<JobBoardDto>>.Failure("An error ocurred while fetching the boards", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<List<PopulatedJobBoardDto>>> GetBoardById(string boardId)
    {
        try
        {
            var ownershipResult = await VerifyBoardOwnership(boardId);

            if (!ownershipResult.Succeeded) return Result<List<PopulatedJobBoardDto>>.Failure(ownershipResult.Error, ownershipResult.ErrorCode);
            
            var board = await _dbContext.JobApplicationBoards
                .Where(board => board.Id == boardId && board.User.Id == userId)
                .ProjectTo<PopulatedJobBoardDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<List<PopulatedJobBoardDto>>.Success(board);
        }
        catch (Exception)
        {
            return Result<List<PopulatedJobBoardDto>>.Failure("An error ocurred while fetching the specified board", (int)HttpStatusCode.InternalServerError);
        }
    }

    private async Task<Result<bool>> VerifyBoardOwnership(string boardId)
    {
        var userResult = await CheckUserExistence();

        if (!userResult.Succeeded) return Result<bool>.Failure(userResult.Error, userResult.ErrorCode);

        var board = await _dbContext.JobApplicationBoards
            .FirstOrDefaultAsync(board => board.Id == boardId  && board.User.Id == userId);

        if (board is null) return Result<bool>.Failure("Board not found", (int)HttpStatusCode.NotFound);  
         
        return Result<bool>.Success(true);  
    }

}