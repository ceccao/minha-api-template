using MinhaApi.DataTransfer.Common;
using MinhaApi.DataTransfer.Requests;
using MinhaApi.DataTransfer.Responses;

namespace MinhaApi.Application.Services;

public interface IProdutoService
{
    Task<ProdutoResponse> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedResult<ProdutoResponse>> ObterTodosAsync(PaginacaoRequest paginacao, CancellationToken cancellationToken = default);
    Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request, CancellationToken cancellationToken = default);
    Task<ProdutoResponse> AtualizarAsync(long id, AtualizarProdutoRequest request, CancellationToken cancellationToken = default);
    Task ExcluirAsync(long id, CancellationToken cancellationToken = default);
}
