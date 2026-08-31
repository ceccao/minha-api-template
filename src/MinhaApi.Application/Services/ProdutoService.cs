using FluentValidation;
using Mapster;
using MinhaApi.CrossCutting.Exceptions;
using MinhaApi.DataTransfer.Common;
using MinhaApi.DataTransfer.Requests;
using MinhaApi.DataTransfer.Responses;
using MinhaApi.Domain.Entities;
using MinhaApi.Domain.Repositories;

namespace MinhaApi.Application.Services;

public class ProdutoService(
    IProdutoRepository repository,
    IValidator<CriarProdutoRequest> criarValidator,
    IValidator<AtualizarProdutoRequest> atualizarValidator) : IProdutoService
{
    public async Task<ProdutoResponse> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var produto = await repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(id);

        // TODO (FASE 17 - IDOR, README §6.3): quando a autenticacao existir, validar
        // aqui se o usuario logado (UsuarioLogadoId/TenantId) tem posse deste recurso
        // antes de retornar - lancar AcessoNegadoException caso contrario.

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task<PagedResult<ProdutoResponse>> ObterTodosAsync(PaginacaoRequest paginacao, CancellationToken cancellationToken = default)
    {
        // Implementacao simples por enquanto: pagina em memoria. Quando o volume real
        // justificar, trocar por paginacao feita direto na query do RepositorioBase
        // (Skip/Take no NHibernate), evitando trazer a tabela inteira pra memoria.
        var produtos = (await repository.ObterTodosAsync(cancellationToken)).ToList();

        var itens = produtos
            .Skip((paginacao.Pagina - 1) * paginacao.TamanhoPagina)
            .Take(paginacao.TamanhoPagina)
            .Adapt<List<ProdutoResponse>>();

        return new PagedResult<ProdutoResponse>(itens, paginacao.Pagina, paginacao.TamanhoPagina, produtos.Count);
    }

    public async Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request, CancellationToken cancellationToken = default)
    {
        var validacao = await criarValidator.ValidateAsync(request, cancellationToken);
        if (!validacao.IsValid)
        {
            throw new EntidadeInvalidaException(validacao.Errors.Select(e => e.ErrorMessage));
        }

        var produto = new Produto(request.Nome, request.Preco);

        await repository.InserirAsync(produto, cancellationToken);

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task<ProdutoResponse> AtualizarAsync(long id, AtualizarProdutoRequest request, CancellationToken cancellationToken = default)
    {
        var validacao = await atualizarValidator.ValidateAsync(request, cancellationToken);
        if (!validacao.IsValid)
        {
            throw new EntidadeInvalidaException(validacao.Errors.Select(e => e.ErrorMessage));
        }

        var produto = await repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(id);

        produto.AtualizarNome(request.Nome);
        produto.AtualizarPreco(request.Preco);

        // NHibernate compara o Version carregado agora contra o que estiver no banco
        // no momento do flush - se outra requisicao mudou o registro nesse meio tempo,
        // estoura StaleObjectStateException, que o ExceptionMiddleware traduz pra 409.
        await repository.AtualizarAsync(produto, cancellationToken);

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task ExcluirAsync(long id, CancellationToken cancellationToken = default)
    {
        var produto = await repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(id);

        // Soft delete (README §5.5): usa o Desativar() da EntidadeBase em vez de
        // excluir a linha de verdade. Exclusao fisica fica reservada para um fluxo
        // separado de anonimizacao/direito ao esquecimento (README §6.4), nao para
        // o DELETE comum da API.
        produto.Desativar();
        await repository.AtualizarAsync(produto, cancellationToken);
    }
}