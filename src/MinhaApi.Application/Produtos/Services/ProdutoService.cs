using Mapster;
using MinhaApi.Application.Produtos.DataTransfer.Requests;
using MinhaApi.Application.Produtos.DataTransfer.Responses;
using MinhaApi.Application.Produtos.Services.Interfaces;
using MinhaApi.Domain.Abstractions;
using MinhaApi.Domain.Produtos.Commands;
using MinhaApi.Domain.Produtos.Entities;
using MinhaApi.Domain.Produtos.Repositories;
using MinhaApi.Domain.Produtos.Repositories.Filters;
using MinhaApi.Domain.Produtos.Services.Interfaces;

namespace MinhaApi.Application.Produtos.Services;

public class ProdutoService(
                            IProdutosService produtosService,
                            IProdutoRepository produtoRepository,
                            IUnitOfWork unitOfWork) : IProdutoService
{
    public async Task<ProdutoResponse> RecuperarAsync(int id, CancellationToken cancellationToken)
    {
        Produto produto = await produtosService.ValidarAsync(id, cancellationToken);
        return produto.Adapt<ProdutoResponse>();
    }

    public async Task<PaginacaoConsulta<ProdutoResponse>> ListarAsync(ListarProdutosRequest request, CancellationToken cancellationToken)
    {
        ProdutoListarFilter filter = request.Adapt<ProdutoListarFilter>();
        IQueryable<Produto> query = produtoRepository.Filtrar(filter);
        PaginacaoConsulta<Produto> resultado = await produtoRepository
            .ListarAsync(
                query,
                request.Qt,
                request.Pg,
                request.CpOrd,
                request.TpOrd,
                cancellationToken);

        return resultado.Adapt<PaginacaoConsulta<ProdutoResponse>>();
    }

    public async Task<ProdutoResponse> InserirAsync(ProdutoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            ProdutoCommand command = request.Adapt<ProdutoCommand>();
            unitOfWork.BeginTransaction();
            Produto produto = await produtosService.InserirAsync(command, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            ProdutoResponse response = produto.Adapt<ProdutoResponse>();
            return response;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ProdutoResponse> EditarAsync(int id, ProdutoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            ProdutoCommand command = request.Adapt<ProdutoCommand>();
            unitOfWork.BeginTransaction();
            Produto produto = await produtosService.EditarAsync(id, command, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            ProdutoResponse response = produto.Adapt<ProdutoResponse>();
            return response;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ProdutoResponse> ExcluirAsync(int id, CancellationToken cancellationToken)
    {
       try
        {
            unitOfWork.BeginTransaction();
            Produto produto = await produtosService.InativarAsync(id, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            ProdutoResponse response = produto.Adapt<ProdutoResponse>();
            return response;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}