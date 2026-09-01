using MinhaApi.Domain.Abstractions;
using MinhaApi.Domain.Produtos.Entities;
using MinhaApi.Domain.Produtos.Repositories.Filters;

namespace MinhaApi.Domain.Produtos.Repositories;

public interface IProdutoRepository : IRepositorioBase<Produto>
{
    Task<IEnumerable<Produto>> ObterComFiltroAsync(ProdutoFilter filtro, CancellationToken cancellationToken = default);
}
