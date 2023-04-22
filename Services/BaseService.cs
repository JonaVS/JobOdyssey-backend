using AutoMapper;
using Microsoft.AspNetCore.Identity;
using JobOdysseyApi.Core;
using JobOdysseyApi.Data;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Services;

public abstract class BaseService
{
    private readonly CoreServiceDependencies _coreServiceDependencies;
    protected readonly UserManager<ApplicationUser> _userManager;
    protected readonly AppDbContext _dbContext;
    protected readonly IMapper _mapper;

    public BaseService(CoreServiceDependencies coreServiceDependencies)
    {
        _coreServiceDependencies = coreServiceDependencies;
        _userManager = _coreServiceDependencies.userManager;
        _dbContext = _coreServiceDependencies.dbContext;
        _mapper = _coreServiceDependencies.mapper;
    }
}