using MinhaApi.Domain.Produtos.Entities;
using MinhaApi.Domain.Produtos.Repositories;
using MinhaApi.Domain.Produtos.Repositories.Filters;
using MinhaApi.Infra.Config;
using NHibernate;
using NHibernate.Linq;

namespace MinhaApi.Infra.Produtos.Repositories;

public class ProdutoRepository(ISession session) : RepositorioBase<Produto>(session), IProdutoRepository
{
    public async Task<IEnumerable<Produto>> ObterComFiltroAsync(ProdutoFilter filtro, CancellationToken cancellationToken = default)
    {
        var query = Session.Query<Produto>();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            query = query.Where(p => p.Nome.Contains(filtro.Nome));
        }

        if (filtro.PrecoMinimo.HasValue)
        {
            query = query.Where(p => p.Preco >= filtro.PrecoMinimo.Value);
        }

        if (filtro.PrecoMaximo.HasValue)
        {
            query = query.Where(p => p.Preco <= filtro.PrecoMaximo.Value);
        }

        if (filtro.Ativo.HasValue)
        {
            query = query.Where(p => p.Ativo == filtro.Ativo.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
