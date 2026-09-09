namespace MinhaApi.Domain.Produtos.Commands;

public class EditarProdutoCommand
{
    public string Nome { get; protected set; } = string.Empty;
    public decimal Preco { get; protected set; }

    public EditarProdutoCommand(string nome, decimal preco)
    {
        Nome = nome;
        Preco = preco;
    }
}