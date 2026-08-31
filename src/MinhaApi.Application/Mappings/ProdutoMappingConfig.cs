using Mapster;
using MinhaApi.DataTransfer.Responses;
using MinhaApi.Domain.Entities;

namespace MinhaApi.Application.Mappings;

// Registrada via TypeAdapterConfig.GlobalSettings.Scan(assembly) na FASE 10 (IoC).
public class ProdutoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // O mapeamento por convencao (mesmos nomes de propriedade: Id, Nome, Preco,
        // Ativo, CriadoEm, AtualizadoEm) ja cobre este caso sozinho. Esta classe existe
        // para centralizar regras futuras quando o DTO divergir da entidade (campos
        // calculados, achatamento de objetos aninhados, etc.).
        config.NewConfig<Produto, ProdutoResponse>();
    }
}
