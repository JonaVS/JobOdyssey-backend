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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		var insertedEntries = this.ChangeTracker.Entries()
							   .Where(x => x.State == EntityState.Added)
							   .Select(x => x.Entity);

		foreach(var insertedEntry in insertedEntries)
		{
			var timeTrackableEntity = insertedEntry as TimeTrackableEntity;
			//If the inserted object is an TimeTrackableEntity.
			if(timeTrackableEntity != null)
			{
				timeTrackableEntity.CreatedAt = DateTimeOffset.UtcNow;
			}
		}

		var modifiedEntries = this.ChangeTracker.Entries()
				   .Where(x => x.State == EntityState.Modified)
				   .Select(x => x.Entity);

		foreach (var modifiedEntry in modifiedEntries)
		{
			//If the modified object is an TimeTrackableEntity.
			var auditableEntity = modifiedEntry as TimeTrackableEntity;
			if (auditableEntity != null)
			{
				auditableEntity.UpdatedAt = DateTimeOffset.UtcNow;
			}
		}

		return base.SaveChangesAsync(cancellationToken);
	}
}