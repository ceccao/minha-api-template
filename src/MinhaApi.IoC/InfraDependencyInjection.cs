using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinhaApi.Domain.Produtos.Repositories;
using MinhaApi.Infra.Config;
using MinhaApi.Infra.Produtos.Repositories;
using NHibernate;

namespace MinhaApi.IoC;

public static class InfraDependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("ConnectionStrings:MySql nao configurado.");

        services.AddSingleton(_ => NHibernateConfiguration.CriarSessionFactory(connectionString));

        // ISession: Scoped, UMA por requisicao HTTP. NUNCA Singleton (README §7.1).
        services.AddScoped(provider => provider.GetRequiredService<ISessionFactory>().OpenSession());

        services.AddScoped<IProdutoRepository, ProdutoRepository>();

        return services;
    }
}
