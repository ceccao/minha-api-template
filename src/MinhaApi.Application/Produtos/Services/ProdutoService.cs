using FluentValidation;
using Mapster;
using MinhaApi.Application.Common;
using MinhaApi.Application.Produtos.DataTransfer.Requests;
using MinhaApi.Application.Produtos.DataTransfer.Responses;
using MinhaApi.Application.Produtos.Services.Interfaces;
using MinhaApi.CrossCutting.Exceptions;
using MinhaApi.Domain.Produtos.Commands;
using MinhaApi.Domain.Produtos.Entities;
using MinhaApi.Domain.Produtos.Repositories;

namespace MinhaApi.Application.Produtos.Services;

public class ProdutoService(
    IProdutoRepository repository,
    IValidator<CriarProdutoCommand> criarValidator,
    IValidator<AtualizarProdutoCommand> atualizarValidator) : IProdutoService
{
    public async Task<ProdutoResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var produto = await repository.RecuperarAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(id);

        // TODO (FASE 17 - IDOR, README §6.3): quando a autenticacao existir, validar
        // aqui se o usuario logado (UsuarioLogadoId/TenantId) tem posse deste recurso
        // antes de retornar - lancar AcessoNegadoException caso contrario.

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task<PagedResult<ProdutoResponse>> ObterTodosAsync(ListarProdutosRequest request, CancellationToken cancellationToken = default)
    {
        // Um unico predicado combinado, com cada condicao opcional "neutralizada"
        // quando o filtro correspondente nao foi informado - nao precisa de
        // biblioteca de combinacao de Expression, o NHibernate resolve isso como
        // um WHERE so com os AND/OR corretos.
        var resultado = await repository.ListarAsync(
            request,
            p =>
                (string.IsNullOrWhiteSpace(request.Nome) || p.Nome.Contains(request.Nome)) &&
                (!request.PrecoMinimo.HasValue || p.Preco >= request.PrecoMinimo.Value) &&
                (!request.PrecoMaximo.HasValue || p.Preco <= request.PrecoMaximo.Value) &&
                (!request.Ativo.HasValue || p.Ativo == request.Ativo.Value),
            cancellationToken);

        var itens = resultado.Itens.Adapt<List<ProdutoResponse>>();

        return new PagedResult<ProdutoResponse>(itens, resultado.Pagina, resultado.TamanhoPagina, resultado.TotalItens);
    }

    public async Task<ProdutoResponse> CriarAsync(CriarProdutoCommand command, CancellationToken cancellationToken = default)
    {
        var validacao = await criarValidator.ValidateAsync(command, cancellationToken);
        if (!validacao.IsValid)
        {
            throw new EntidadeInvalidaException(validacao.Errors.Select(e => e.ErrorMessage));
        }

        var produto = new Produto(command.Nome, command.Preco);

        await repository.InserirAsync(produto, cancellationToken);

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task<ProdutoResponse> AtualizarAsync(AtualizarProdutoCommand command, CancellationToken cancellationToken = default)
    {
        var validacao = await atualizarValidator.ValidateAsync(command, cancellationToken);
        if (!validacao.IsValid)
        {
            throw new EntidadeInvalidaException(validacao.Errors.Select(e => e.ErrorMessage));
        }

        var produto = await repository.RecuperarAsync(command.Id, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(command.Id);

        produto.AtualizarNome(command.Nome);
        produto.AtualizarPreco(command.Preco);

        await repository.EditarAsync(produto, cancellationToken);

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var produto = await repository.RecuperarAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(id);

        produto.Desativar();
        await repository.EditarAsync(produto, cancellationToken);
    }

    public async Task<Result<ProdutoResponse>> AtualizarPrecoComRetryAsync(
        int id, decimal novoPreco, int maxTentativas = 3, CancellationToken cancellationToken = default)
    {
        for (var tentativa = 1; tentativa <= maxTentativas; tentativa++)
        {
            try
            {
                var produto = await repository.RecuperarAsync(id, cancellationToken)
                    ?? throw new NaoEncontradoException<Produto>(id);

                produto.AtualizarPreco(novoPreco);
                await repository.EditarAsync(produto, cancellationToken);

                return Result.Ok(produto.Adapt<ProdutoResponse>());
            }
            catch (ConflitoException)
            {
                if (tentativa == maxTentativas)
                {
                    return Result.Falha<ProdutoResponse>(
                        $"Não foi possível atualizar o preço após {maxTentativas} tentativas devido a conflitos concorrentes simultâneos.");
                }
            }
        }

        return Result.Falha<ProdutoResponse>("Erro inesperado ao atualizar o preço.");
    }
}