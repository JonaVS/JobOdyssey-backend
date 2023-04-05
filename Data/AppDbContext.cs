using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JobOdysseyApi.Data;

public class AppDbContext : IdentityDbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}