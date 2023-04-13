using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser> 
{
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}