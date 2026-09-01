using FluentNHibernate.Mapping;
using MinhaApi.Domain.Produtos.Entities;

namespace MinhaApi.Infra.Produtos.Mappings;

public class ProdutoMap : ClassMap<Produto>
{
    public ProdutoMap()
    {
        Table("produto");
        Id(x => x.Id).Column("id").GeneratedBy.Identity();
        Map(x => x.Nome).Column("nome").Length(100).Not.Nullable();
        Map(x => x.Preco).Column("preco").Precision(18).Scale(2).Not.Nullable();
        Map(x => x.Ativo).Column("ativo").Not.Nullable();
        Map(x => x.CriadoEm).Column("criado_em").Not.Nullable();
        Map(x => x.AtualizadoEm).Column("atualizado_em");
        Version(x => x.Version).Column("version"); // concorrencia otimista (lost update), README §7.3
    }
}
