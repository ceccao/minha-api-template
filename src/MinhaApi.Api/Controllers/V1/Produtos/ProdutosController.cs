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
    // Ex: GET /api/v1/produtos?qt=20&pg=1&cpOrd=Nome&tpOrd=Descendente&nome=tec
    [HttpGet]
    [ProducesResponseType<PagedResult<ProdutoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos([FromQuery] ListarProdutosRequest request, CancellationToken ct)
        => Ok(await service.ObterTodosAsync(request, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProdutoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken ct)
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

    [HttpPut("{id:int}")]
    [ProducesResponseType<ProdutoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(int id, AtualizarProdutoRequest request, CancellationToken ct)
    {
        var command = new AtualizarProdutoCommand(id, request.Nome, request.Preco);
        return Ok(await service.AtualizarAsync(command, ct));
    }

    [HttpPatch("{id:int}/preco")]
    [ProducesResponseType<ProdutoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AtualizarPreco(int id, AtualizarPrecoRequest request, CancellationToken ct)
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

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken ct)
    {
        await service.ExcluirAsync(id, ct);
        return NoContent();
    }
}