using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using MinhaApi.Application.Produtos.Profiles;
using MinhaApi.Application.Produtos.Services;
using MinhaApi.Application.Produtos.Services.Interfaces;
using MinhaApi.Application.Produtos.Validators;

namespace MinhaApi.IoC;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProdutoService, ProdutoService>();

        services.AddValidatorsFromAssemblyContaining<CriarProdutoCommandValidator>();

        TypeAdapterConfig.GlobalSettings.Scan(typeof(ProdutoMappingConfig).Assembly);

        return services;
    }
}
