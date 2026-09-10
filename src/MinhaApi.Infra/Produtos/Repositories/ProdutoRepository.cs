using MinhaApi.Domain.Produtos.Entities;
using MinhaApi.Domain.Produtos.Repositories;
using MinhaApi.Domain.Produtos.Repositories.Filters;
using MinhaApi.Infra.Config;
using NHibernate;

namespace MinhaApi.Infra.Produtos.Repositories;

public class ProdutoRepository(ISession session) : RepositorioBase<Produto>(session), IProdutoRepository
{
    public IQueryable<Produto> Filtrar(ProdutoListarFilter filtro)
    {
        var query = Session.Query<Produto>();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            query = query.Where(p => p.Nome.Contains(filtro.Nome));
        }

        return query;
    }
}