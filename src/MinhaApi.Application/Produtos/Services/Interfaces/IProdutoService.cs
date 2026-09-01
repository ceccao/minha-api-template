using MinhaApi.Application.Common;
using MinhaApi.Application.Produtos.DataTransfer.Responses;
using MinhaApi.Domain.Produtos.Commands;

namespace MinhaApi.Application.Produtos.Services.Interfaces;

public interface IProdutoService
{
    Task<ProdutoResponse> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedResult<ProdutoResponse>> ObterTodosAsync(PaginacaoRequest paginacao, CancellationToken cancellationToken = default);
    Task<ProdutoResponse> CriarAsync(CriarProdutoCommand command, CancellationToken cancellationToken = default);
    Task<ProdutoResponse> AtualizarAsync(AtualizarProdutoCommand command, CancellationToken cancellationToken = default);
    Task ExcluirAsync(long id, CancellationToken cancellationToken = default);

    // Exemplo de uso do Result Pattern (README §5.1/§8.2): fluxo com retry em
    // conflito de concorrencia otimista - o chamador decide o que fazer com
    // Sucesso/Falha, em vez de receber uma exception de conflito direto.
    Task<Result<ProdutoResponse>> AtualizarPrecoComRetryAsync(long id, decimal novoPreco, int maxTentativas = 3, CancellationToken cancellationToken = default);
}
