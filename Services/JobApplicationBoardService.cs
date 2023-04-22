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

    public async Task<Result<PopulatedJobBoardDto>> GetBoardById(string boardId)
    {
        try
        {
            var ownershipResult = await VerifyBoardOwnership(boardId, populate: true);

            if (!ownershipResult.Succeeded) return Result<PopulatedJobBoardDto>.Failure(ownershipResult.Error, ownershipResult.ErrorCode);

            return Result<PopulatedJobBoardDto>.Success(_mapper.Map<PopulatedJobBoardDto>(ownershipResult.Data));
        }
        catch (Exception)
        {
            return Result<PopulatedJobBoardDto>.Failure("An error ocurred while fetching the specified board", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<JobBoardDto>> UpdateBoard(string boardId, UpdateJobBoardDto updateData)
    {
        try
        {
            var ownershipResult = await VerifyBoardOwnership(boardId, populate: false);

            if (!ownershipResult.Succeeded) return Result<JobBoardDto>.Failure(ownershipResult.Error, ownershipResult.ErrorCode);

            var targetBoard = ownershipResult.Data;

            //For now, only the board name can be updated
            targetBoard!.Name = updateData.Name;

            await _dbContext.SaveChangesAsync();

            return Result<JobBoardDto>.Success(_mapper.Map<JobBoardDto>(targetBoard));
        }
        catch (Exception)
        {
            return Result<JobBoardDto>.Failure("An error ocurred while updating the specified board", (int)HttpStatusCode.InternalServerError);    
        }
    }

    public async Task<Result> DeleteBoard(string boardId)
    {
        try
        {
            var ownershipResult = await VerifyBoardOwnership(boardId, populate: false);

            if (!ownershipResult.Succeeded) return Result.Failure(ownershipResult.Error, ownershipResult.ErrorCode);

            var targetBoard = ownershipResult.Data;

            _dbContext.JobApplicationBoards.Remove(targetBoard!);

            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure("An error ocurred while deleting the specified board", (int)HttpStatusCode.InternalServerError);
        }
    }

    private async Task<Result<JobApplicationBoard>> VerifyBoardOwnership(string boardId, bool populate = false)
    {
        var userResult = await CheckUserExistence();

        if (!userResult.Succeeded) return Result<JobApplicationBoard>.Failure(userResult.Error, userResult.ErrorCode);

        JobApplicationBoard? board;

        if (populate)
        {
            board = await _dbContext.JobApplicationBoards
                .Where(board => board.Id == boardId && board.User.Id == userId)
                .Include(x => x.JobApplications)
                .FirstOrDefaultAsync(); 
        }
        else
        {
            board = await _dbContext.JobApplicationBoards
                .FirstOrDefaultAsync(board => board.Id == boardId && board.User.Id == userId);
        }

        if (board is null) return Result<JobApplicationBoard>.Failure("The specified board does not exist or is inaccessible.", (int)HttpStatusCode.NotFound);

        return Result<JobApplicationBoard>.Success(board);
    }

}