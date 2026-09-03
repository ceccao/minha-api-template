using MinhaApi.Application.Common;
using MinhaApi.Application.Produtos.DataTransfer.Requests;
using MinhaApi.Application.Produtos.DataTransfer.Responses;
using MinhaApi.Domain.Produtos.Commands;

namespace MinhaApi.Application.Produtos.Services.Interfaces;

public interface IProdutoService
{
    Task<ProdutoResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<ProdutoResponse>> ObterTodosAsync(ListarProdutosRequest request, CancellationToken cancellationToken = default);
    Task<ProdutoResponse> CriarAsync(CriarProdutoCommand command, CancellationToken cancellationToken = default);
    Task<ProdutoResponse> AtualizarAsync(AtualizarProdutoCommand command, CancellationToken cancellationToken = default);
    Task ExcluirAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<ProdutoResponse>> AtualizarPrecoComRetryAsync(int id, decimal novoPreco, int maxTentativas = 3, CancellationToken cancellationToken = default);
}