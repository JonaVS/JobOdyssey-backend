using AutoMapper;
using JobOdysseyApi.Data;
using JobOdysseyApi.Models;
using Microsoft.AspNetCore.Identity;

namespace JobOdysseyApi.Core;

public class CoreServiceDependencies
{
    public readonly UserManager<ApplicationUser> userManager;
    public readonly AppDbContext dbContext;
    public readonly IMapper mapper;

    public CoreServiceDependencies(UserManager<ApplicationUser> userManger, AppDbContext dbContext, IMapper mapper)
    {
        this.userManager = userManger;
        this.dbContext = dbContext;
        this.mapper = mapper;
    }
}