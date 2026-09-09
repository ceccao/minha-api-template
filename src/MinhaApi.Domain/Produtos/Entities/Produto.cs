using MinhaApi.Domain.Common;

namespace MinhaApi.Domain.Produtos.Entities;

public class Produto : EntidadeBase
{
    public virtual string Nome { get; protected set; } = string.Empty;
    public virtual decimal Preco { get; protected set; }

    protected Produto() { }

    public Produto(string nome, decimal preco)
    {
        ValidarNome(nome);
        ValidarPreco(preco);

        Nome = nome;
        Preco = preco;
    }

    public virtual void SetNome(string nome)
    {
        ValidarNome(nome);

        Nome = nome;
        AtualizadoEm = DateTime.UtcNow;
    }

    public virtual void SetPreco(decimal preco)
    {
        ValidarPreco(preco);

        Preco = preco;
        AtualizadoEm = DateTime.UtcNow;
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório.", nameof(nome));

        // Corrigido: era "&&" (nunca disparava - nenhuma string tem tamanho < 3 E > 100
        // ao mesmo tempo, entao a regra de tamanho nunca era aplicada). O correto e "||".
        if (nome.Length < 3 || nome.Length > 100)
            throw new ArgumentException("Nome deve ter no mínimo 3 e máximo 100 caracteres.", nameof(nome));
    }

    private static void ValidarPreco(decimal preco)
    {
        if (preco < 0)
            throw new ArgumentException("Preço não pode ser negativo.", nameof(preco));
    }
}