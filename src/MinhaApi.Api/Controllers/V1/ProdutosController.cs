using Microsoft.AspNetCore.Mvc;
using MinhaApi.Application.Services;
using MinhaApi.DataTransfer.Common;
using MinhaApi.DataTransfer.Requests;
using MinhaApi.DataTransfer.Responses;

namespace MinhaApi.Api.Controllers.V1;

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
        var produto = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType<ProdutoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(long id, AtualizarProdutoRequest request, CancellationToken ct)
        => Ok(await service.AtualizarAsync(id, request, ct));

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(long id, CancellationToken ct)
    {
        await service.ExcluirAsync(id, ct);
        return NoContent();
    }
}
