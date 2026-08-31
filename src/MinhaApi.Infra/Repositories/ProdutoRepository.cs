using MinhaApi.Domain.Entities;
using MinhaApi.Domain.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace MinhaApi.Infra.Repositories;

public class ProdutoRepository(ISession session) : RepositorioBase<Produto>(session), IProdutoRepository
{
    public async Task<IEnumerable<Produto>> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
        => await Session.Query<Produto>()
            .Where(p => p.Nome.Contains(nome))
            .ToListAsync(cancellationToken);
}
