using FluentValidation;
using Mapster;
using MinhaApi.Application.Common;
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
    public async Task<ProdutoResponse> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var produto = await repository.RecuperarAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(id);

        // TODO (FASE 17 - IDOR, README §6.3): quando a autenticacao existir, validar
        // aqui se o usuario logado (UsuarioLogadoId/TenantId) tem posse deste recurso
        // antes de retornar - lancar AcessoNegadoException caso contrario.

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task<PagedResult<ProdutoResponse>> ObterTodosAsync(PaginacaoRequest paginacao, CancellationToken cancellationToken = default)
    {
        var produtos = (await repository.ObterTodosAsync(cancellationToken)).ToList();

        var itens = produtos
            .Skip((paginacao.Pagina - 1) * paginacao.TamanhoPagina)
            .Take(paginacao.TamanhoPagina)
            .Adapt<List<ProdutoResponse>>();

        return new PagedResult<ProdutoResponse>(itens, paginacao.Pagina, paginacao.TamanhoPagina, produtos.Count);
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

        // NHibernate compara o Version carregado agora contra o que estiver no banco
        // no momento do flush - se outra requisicao mudou o registro nesse meio tempo,
        // o RepositorioBase (Infra) traduz isso em ConflitoException, que o
        // ExceptionMiddleware converte pra 409.
        await repository.AtualizarAsync(produto, cancellationToken);

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task ExcluirAsync(long id, CancellationToken cancellationToken = default)
    {
        var produto = await repository.RecuperarAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(id);

        // Soft delete (README §5.5): usa o Desativar() da EntidadeBase em vez de
        // excluir a linha de verdade.
        produto.Desativar();
        await repository.AtualizarAsync(produto, cancellationToken);
    }

    public async Task<Result<ProdutoResponse>> AtualizarPrecoComRetryAsync(
        long id, decimal novoPreco, int maxTentativas = 3, CancellationToken cancellationToken = default)
    {
        for (var tentativa = 1; tentativa <= maxTentativas; tentativa++)
        {
            try
            {
                var produto = await repository.RecuperarAsync(id, cancellationToken)
                    ?? throw new NaoEncontradoException<Produto>(id);

                produto.AtualizarPreco(novoPreco);
                await repository.AtualizarAsync(produto, cancellationToken);

                return Result.Ok(produto.Adapt<ProdutoResponse>());
            }
            catch (ConflitoException)
            {
                // Outra requisicao mudou o preco no meio do caminho. Se ainda sobrar
                // tentativa, o loop recarrega o estado mais recente (RecuperarAsync de
                // novo) e tenta outra vez; senao, desiste e devolve Falha - o chamador
                // decide o proximo passo (isso e o Result Pattern na pratica, README §5.1).
                if (tentativa == maxTentativas)
                {
                    return Result.Falha<ProdutoResponse>(
                        $"Não foi possível atualizar o preço após {maxTentativas} tentativas devido a conflitos concorrentes simultâneos.");
                }
            }
        }

        // Inalcancavel (o loop sempre retorna ou cai no "if" acima), mas o compilador
        // exige um caminho de retorno.
        return Result.Falha<ProdutoResponse>("Erro inesperado ao atualizar o preço.");
    }
}
