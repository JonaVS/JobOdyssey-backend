using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser> 
{
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<JobApplicationBoard> JobApplicationBoards { get; set; } = null!;
    public DbSet<JobApplication> JobApplications { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(DotNetEnv.Env.GetString("DB_CONNECTION"));

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresEnum<JobApplicationStatus>();
        base.OnModelCreating(builder);
    }
}