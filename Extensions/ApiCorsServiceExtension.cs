public static class ApiCorsServiceExtensions
{
    public static IServiceCollection AddApiCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("ApiCorsConfig", builder =>
            {
                //This is just for dev mode. Needs to be updated later on.
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        return services;
    }
}