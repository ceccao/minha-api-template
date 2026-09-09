namespace MinhaApi.Application.Produtos.DataTransfer.Requests;

public class AtualizarProdutoRequest
{
    public required string Nome { get; set; }
    public decimal Preco { get; set; }
}