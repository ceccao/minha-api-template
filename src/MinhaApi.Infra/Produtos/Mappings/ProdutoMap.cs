using FluentNHibernate.Mapping;
using MinhaApi.Domain.Produtos.Entities;

namespace MinhaApi.Infra.Produtos.Mappings;

public class ProdutoMap : ClassMap<Produto>
{
    public ProdutoMap()
    {
        Table("PRODUTO");

        Id(x => x.Id).Column("ID").GeneratedBy.Identity();
        
        Map(x => x.Nome).Column("NOME").Length(100).Not.Nullable();
        Map(x => x.Preco).Column("PRECO").Precision(18).Scale(2).Not.Nullable();
        Map(x => x.Situacao).Column("SITUACAO").CustomType<int>().Not.Nullable();
    }
}