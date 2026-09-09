using MinhaApi.Domain.Produtos.Commands;
using MinhaApi.Domain.Produtos.Entities;

namespace MinhaApi.Domain.Produtos.Services.Interfaces;

public interface IProdutosService
{
    // Retorno nullable de proposito: o Domain nao pode referenciar o CrossCutting
    // (regra de dependencia, README §2.1), entao nao pode lancar NaoEncontradoException
    // daqui. "null" sinaliza "nao encontrado" - quem traduz isso pra exception e a
    // Application (que ja conhece os dois lados).
    Task<Produto?> RecuperarAsync(int id, CancellationToken cancellationToken);
    Task<Produto> CriarAsync(InserirProdutoCommand command, CancellationToken cancellationToken);
    Task<Produto?> EditarAsync(int id, EditarProdutoCommand command, CancellationToken cancellationToken);
    Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken);
}