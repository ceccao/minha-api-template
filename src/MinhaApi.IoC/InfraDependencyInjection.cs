using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinhaApi.Application.Abstractions;
using MinhaApi.Domain.Repositories;
using MinhaApi.Infra.Persistence;
using MinhaApi.Infra.Repositories;
using MinhaApi.Infra.Services;
using NHibernate;

namespace MinhaApi.IoC;

public static class InfraDependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("ConnectionStrings:MySql nao configurado.");

        // ISessionFactory: Singleton, cara de construir (README §7.1/9.3).
        services.AddSingleton(_ => NHibernateConfiguration.CriarSessionFactory(connectionString));

        // ISession: Scoped, UMA por requisicao HTTP. NUNCA Singleton - erro classico
        // de quem vem do Entity Framework (README §7.1). Registrar como Singleton
        // aqui vazaria dados entre usuarios e causaria memory leak.
        services.AddScoped(provider => provider.GetRequiredService<ISessionFactory>().OpenSession());

        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
