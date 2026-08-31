namespace MinhaApi.Api.Configuration;

public static class CorsConfiguration
{
    public const string PoliticaPadrao = "Padrao";

    public static IServiceCollection AddCorsConfigurada(this IServiceCollection services, IConfiguration configuration)
    {
        var origensPermitidas = configuration.GetSection("Cors:OrigensPermitidas").Get<string[]>() ?? [];

        services.AddCors(options =>
            options.AddPolicy(PoliticaPadrao, policy => policy
                .WithOrigins(origensPermitidas)
                .AllowAnyHeader()
                .WithExposedHeaders("X-Correlation-Id") // o front precisa ler esse header
                .AllowAnyMethod()));

        return services;
    }
}
