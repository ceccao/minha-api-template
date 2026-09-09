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
using MinhaApi.Domain.Produtos.Services.Interfaces;

namespace MinhaApi.Application.Produtos.Services;

public class ProdutoService(
    IProdutosService produtosService,
    IProdutoRepository repository,
    IValidator<CriarProdutoRequest> criarValidator,
    IValidator<AtualizarProdutoRequest> atualizarValidator) : IProdutoService
{
    public async Task<ProdutoResponse> RecuperarAsync(int id, CancellationToken cancellationToken)
    {
        var produto = await produtosService.RecuperarAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(id);

        // TODO (FASE 17 - IDOR, README §6.3): quando a autenticacao existir, validar
        // aqui se o usuario logado (UsuarioLogadoId/TenantId) tem posse deste recurso.

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task<PagedResult<ProdutoResponse>> ListarAsync(ListarProdutosRequest request, CancellationToken cancellationToken)
    {
        // Consulta simples, sem regra de negocio - vai direto no repositorio (Infra),
        // sem passar pelo Domain Service. Um unico predicado combinado, com cada
        // condicao opcional neutralizada quando o filtro nao foi informado.
        var resultado = await repository.ListarAsync(
            request,
            p =>
                (string.IsNullOrWhiteSpace(request.Nome) || p.Nome.Contains(request.Nome)) &&
                (!request.PrecoMinimo.HasValue || p.Preco >= request.PrecoMinimo.GetValueOrDefault()) &&
                (!request.PrecoMaximo.HasValue || p.Preco <= request.PrecoMaximo.GetValueOrDefault()) &&
                (!request.Ativo.HasValue || p.Ativo == request.Ativo.GetValueOrDefault()),
            cancellationToken);

        var itens = resultado.Itens.Adapt<List<ProdutoResponse>>();

        return new PagedResult<ProdutoResponse>(itens, resultado.Pagina, resultado.TamanhoPagina, resultado.TotalItens);
    }

    public async Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request, CancellationToken cancellationToken)
    {
        var validacao = await criarValidator.ValidateAsync(request, cancellationToken);
        if (!validacao.IsValid)
        {
            throw new EntidadeInvalidaException(validacao.Errors.Select(e => e.ErrorMessage));
        }

        var command = request.Adapt<InserirProdutoCommand>();
        var produto = await produtosService.CriarAsync(command, cancellationToken);

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task<ProdutoResponse> EditarAsync(int id, AtualizarProdutoRequest request, CancellationToken cancellationToken)
    {
        var validacao = await atualizarValidator.ValidateAsync(request, cancellationToken);
        if (!validacao.IsValid)
        {
            throw new EntidadeInvalidaException(validacao.Errors.Select(e => e.ErrorMessage));
        }

        // Sem mapeamento automatico aqui de proposito: o Id vem da rota, nao do
        // Request, entao o Command e montado direto (ver nota no ProdutoProfile).
        var command = new EditarProdutoCommand(request.Nome, request.Preco);

        var produto = await produtosService.EditarAsync(id, command, cancellationToken)
            ?? throw new NaoEncontradoException<Produto>(id);

        return produto.Adapt<ProdutoResponse>();
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken)
    {
        var excluiu = await produtosService.ExcluirAsync(id, cancellationToken);
        if (!excluiu)
        {
            throw new NaoEncontradoException<Produto>(id);
        }
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

                produto.SetPreco(novoPreco);
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