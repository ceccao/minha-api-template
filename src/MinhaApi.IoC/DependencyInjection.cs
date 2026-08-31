using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MinhaApi.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddIoC(this IServiceCollection services, IConfiguration config)
        => services
            .AddCrossCutting(config)
            .AddInfra(config)
            .AddApplication();
}
