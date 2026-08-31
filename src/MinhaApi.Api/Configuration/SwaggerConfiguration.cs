using Microsoft.OpenApi;

namespace MinhaApi.Api.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfigurado(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MinhaApi",
                Version = "v1"
            });
        });

        return services;
    }
}
