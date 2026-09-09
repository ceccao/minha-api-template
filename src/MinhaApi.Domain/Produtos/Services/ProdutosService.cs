using MinhaApi.Domain.Produtos.Commands;
using MinhaApi.Domain.Produtos.Entities;
using MinhaApi.Domain.Produtos.Repositories;
using MinhaApi.Domain.Produtos.Services.Interfaces;

namespace MinhaApi.Domain.Produtos.Services;

public class ProdutosService(IProdutoRepository produtoRepository) : IProdutosService
{
    public async Task<Produto?> RecuperarAsync(int id, CancellationToken cancellationToken)
        => await produtoRepository.RecuperarAsync(id, cancellationToken);

    public async Task<Produto> CriarAsync(InserirProdutoCommand command, CancellationToken cancellationToken)
    {
        // Produto(nome, preco) ja valida os invariantes (obrigatorio, tamanho 3-100,
        // preco nao-negativo) e lanca ArgumentException se algo for invalido - essa
        // e a "logica que mora no dominio". A traducao de ArgumentException pro tipo
        // que vira 400 (EntidadeInvalidaException) acontece na Application, ja que
        // o Domain nao pode referenciar o CrossCutting.
        var produto = new Produto(command.Nome, command.Preco);

        await produtoRepository.InserirAsync(produto, cancellationToken);

        return produto;
    }

    public async Task<Produto?> EditarAsync(int id, EditarProdutoCommand command, CancellationToken cancellationToken)
    {
        var produto = await produtoRepository.RecuperarAsync(id, cancellationToken);
        if (produto is null)
        {
            return null;
        }

        produto.SetNome(command.Nome);
        produto.SetPreco(command.Preco);

        // Se outra requisicao mudou o registro nesse meio tempo, o RepositorioBase
        // (Infra) traduz StaleObjectStateException em ConflitoException - esse tipo
        // ja e do CrossCutting, mas quem lanca e a Infra (que pode referencia-lo),
        // nao o Domain. A excecao so atravessa o Domain Service sem ser tocada.
        await produtoRepository.EditarAsync(produto, cancellationToken);

        return produto;
    }

    public async Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken)
    {
        var produto = await produtoRepository.RecuperarAsync(id, cancellationToken);
        if (produto is null)
        {
            return false;
        }

        // Soft delete (README §5.5): Desativar() da EntidadeBase em vez de excluir
        // a linha de verdade.
        produto.Desativar();
        await produtoRepository.EditarAsync(produto, cancellationToken);

        return true;
    }
}