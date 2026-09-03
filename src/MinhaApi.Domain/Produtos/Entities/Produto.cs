using MinhaApi.Domain.Common;

namespace MinhaApi.Domain.Produtos.Entities;

public class Produto : EntidadeBase
{
    public virtual string Nome { get; protected set; } = string.Empty;
    public virtual decimal Preco { get; protected set; }

    // Construtor sem parametros: exigencia do NHibernate para instanciar via proxy.
    protected Produto()
    {
    }

    public Produto(string nome, decimal preco)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório.", nameof(nome));

        if (preco < 0)
            throw new ArgumentException("Preço não pode ser negativo.", nameof(preco));

        Nome = nome;
        Preco = preco;
    }

    public virtual void AtualizarPreco(decimal novoPreco)
    {
        if (novoPreco < 0)
            throw new ArgumentException("Preço não pode ser negativo.", nameof(novoPreco));

        Preco = novoPreco;
        AtualizadoEm = DateTime.UtcNow;
    }

    public virtual void AtualizarNome(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new ArgumentException("Nome do produto é obrigatório.", nameof(novoNome));

        Nome = novoNome;
        AtualizadoEm = DateTime.UtcNow;
    }
}