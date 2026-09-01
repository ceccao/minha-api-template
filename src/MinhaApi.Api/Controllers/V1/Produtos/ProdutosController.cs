using Mapster;
using Microsoft.AspNetCore.Mvc;
using MinhaApi.Application.Common;
using MinhaApi.Application.Produtos.DataTransfer.Requests;
using MinhaApi.Application.Produtos.DataTransfer.Responses;
using MinhaApi.Application.Produtos.Services.Interfaces;
using MinhaApi.Domain.Produtos.Commands;

namespace MinhaApi.Api.Controllers.V1.Produtos;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ProdutosController(IProdutoService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<ProdutoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos([FromQuery] PaginacaoRequest paginacao, CancellationToken ct)
        => Ok(await service.ObterTodosAsync(paginacao, ct));

    [HttpGet("{id:long}")]
    [ProducesResponseType<ProdutoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(long id, CancellationToken ct)
        => Ok(await service.ObterPorIdAsync(id, ct));

    [HttpPost]
    [ProducesResponseType<ProdutoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar(CriarProdutoRequest request, CancellationToken ct)
    {
        var command = request.Adapt<CriarProdutoCommand>();
        var produto = await service.CriarAsync(command, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType<ProdutoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(long id, AtualizarProdutoRequest request, CancellationToken ct)
    {
        var command = new AtualizarProdutoCommand(id, request.Nome, request.Preco);
        return Ok(await service.AtualizarAsync(command, ct));
    }

    // Exemplo de endpoint que consome um fluxo baseado em Result<T> (README §5.1/§8.2):
    // o Service tenta novamente sozinho em caso de conflito de concorrencia, e so aqui,
    // na borda da API, o resultado (Sucesso/Falha) vira de fato um status HTTP.
    [HttpPatch("{id:long}/preco")]
    [ProducesResponseType<ProdutoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AtualizarPreco(long id, AtualizarPrecoRequest request, CancellationToken ct)
    {
        var resultado = await service.AtualizarPrecoComRetryAsync(id, request.NovoPreco, cancellationToken: ct);

        if (!resultado.Sucesso)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = resultado.Erro
            });
        }

        return Ok(resultado.Valor);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(long id, CancellationToken ct)
    {
        await service.ExcluirAsync(id, ct);
        return NoContent();
    }
}
