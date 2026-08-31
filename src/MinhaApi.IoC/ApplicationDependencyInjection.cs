using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using MinhaApi.Application.Mappings;
using MinhaApi.Application.Services;
using MinhaApi.Application.Validators;

namespace MinhaApi.IoC;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProdutoService, ProdutoService>();

        // Escaneia o assembly da Application procurando classes AbstractValidator<T>
        // (CriarProdutoRequestValidator, AtualizarProdutoRequestValidator, e as que
        // vierem depois) e registra todas automaticamente.
        services.AddValidatorsFromAssemblyContaining<CriarProdutoRequestValidator>();

        // Usamos .Adapt<T>() estatico (nao IMapper injetado) nos Services, entao so
        // precisamos popular a config global escaneando as classes IRegister
        // (ProdutoMappingConfig e as que vierem depois).
        TypeAdapterConfig.GlobalSettings.Scan(typeof(ProdutoMappingConfig).Assembly);

        return services;
    }
}
