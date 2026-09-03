using MinhaApi.Domain.Abstractions;
using MinhaApi.Domain.Produtos.Entities;

namespace MinhaApi.Domain.Produtos.Repositories;

// Vazia de proposito por enquanto: RecuperarAsync(predicado) e ListarAsync
// (paginado/ordenado, com filtro opcional) do IRepositorioBase<T> ja cobrem os
// casos de consulta que existiam aqui antes (ObterComFiltroAsync). Ganha metodos
// proprios quando surgir uma consulta que genuinamente nao da pra expressar como
// predicado simples.
public interface IProdutoRepository : IRepositorioBase<Produto>
{
}
