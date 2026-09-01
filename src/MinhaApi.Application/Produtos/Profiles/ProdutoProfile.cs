using Mapster;
using MinhaApi.Application.Produtos.DataTransfer.Responses;
using MinhaApi.Domain.Produtos.Entities;

namespace MinhaApi.Application.Produtos.Profiles;

// Registrada via TypeAdapterConfig.GlobalSettings.Scan(assembly) na FASE 10 (IoC).
public class ProdutoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Produto -> ProdutoResponse ja e coberto por convencao (mesmos nomes de
        // propriedade). Request -> Command tambem e coberto por convencao quando os
        // campos batem (CriarProdutoRequest -> CriarProdutoCommand). Esta classe fica
        // aqui pronta pra quando algum desses mapeamentos precisar de regra customizada.
        config.NewConfig<Produto, ProdutoResponse>();
    }
}
