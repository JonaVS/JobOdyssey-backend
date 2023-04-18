using Microsoft.EntityFrameworkCore;
using Npgsql;
using JobOdysseyApi.Data;
using JobOdysseyApi.Models;

public static class PostgresServiceExtensions
{
    public async static Task<IServiceCollection> AddPostgresDatabase(this IServiceCollection services)
    {

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DotNetEnv.Env.GetString("DB_CONNECTION"));
        dataSourceBuilder.MapEnum<JobApplicationStatus>();
        await using var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));

        return services;
    }
}