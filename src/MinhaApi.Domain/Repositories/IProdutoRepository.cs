using MinhaApi.Domain.Abstractions;
using MinhaApi.Domain.Entities;

namespace MinhaApi.Domain.Repositories;

public interface IProdutoRepository : IRepositorioBase<Produto>
{
    Task<IEnumerable<Produto>> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
}
