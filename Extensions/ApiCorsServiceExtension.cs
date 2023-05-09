public static class ApiCorsServiceExtensions
{
    public static IServiceCollection AddApiCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("ApiCorsConfig", builder =>
            {
                //This is just for dev mode. Needs to be updated later on.
                builder.WithOrigins("http://localhost:3000")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            });
        });

        return services;
    }
}