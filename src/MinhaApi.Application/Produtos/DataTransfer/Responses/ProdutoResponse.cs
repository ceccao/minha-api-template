namespace MinhaApi.Application.Produtos.DataTransfer.Responses;

public record ProdutoResponse(
    int Id,
    string Nome,
    decimal Preco,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);