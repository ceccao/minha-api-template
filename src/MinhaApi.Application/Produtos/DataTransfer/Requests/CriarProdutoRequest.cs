namespace MinhaApi.Application.Produtos.DataTransfer.Requests;
 
public class CriarProdutoRequest
{
    public required string Nome { get; set; }
    public decimal Preco { get; set; }
}
 