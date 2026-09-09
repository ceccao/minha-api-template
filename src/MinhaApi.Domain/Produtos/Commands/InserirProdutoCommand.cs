namespace MinhaApi.Domain.Produtos.Commands;

public class InserirProdutoCommand(string nome, decimal preco)
{
    public string Nome { get; protected set; } = nome;
    public decimal Preco { get; protected set; } = preco;
}