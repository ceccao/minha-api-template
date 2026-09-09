using MinhaApi.Application.Common;
using MinhaApi.Application.Produtos.DataTransfer.Requests;
using MinhaApi.Application.Produtos.DataTransfer.Responses;

namespace MinhaApi.Application.Produtos.Services.Interfaces;

public interface IProdutoService
{
    Task<ProdutoResponse> RecuperarAsync(int id, CancellationToken cancellationToken);
    Task<PagedResult<ProdutoResponse>> ListarAsync(ListarProdutosRequest request, CancellationToken cancellationToken);
    Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request, CancellationToken cancellationToken);
    Task<ProdutoResponse> EditarAsync(int id, AtualizarProdutoRequest request, CancellationToken cancellationToken);
    Task ExcluirAsync(int id, CancellationToken cancellationToken);

    // Continua fora do fluxo Domain Service de proposito - e um caso especial de
    // retry/resiliencia (README §5.1/§8.2, Result Pattern), nao uma regra de negocio.
    Task<Result<ProdutoResponse>> AtualizarPrecoComRetryAsync(int id, decimal novoPreco, int maxTentativas = 3, CancellationToken cancellationToken = default);
}