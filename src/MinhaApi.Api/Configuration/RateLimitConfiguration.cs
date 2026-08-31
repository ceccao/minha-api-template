using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace MinhaApi.Api.Configuration;

public static class RateLimitConfiguration
{
    public const string PoliticaPadrao = "Padrao";

    public static IServiceCollection AddRateLimitConfigurado(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter(PoliticaPadrao, limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0;
            });
        });

        return services;
    }
}
