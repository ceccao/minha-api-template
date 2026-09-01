namespace MinhaApi.Application.Produtos.DataTransfer.Responses;

public record ProdutoResponse(
    long Id,
    string Nome,
    decimal Preco,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);
