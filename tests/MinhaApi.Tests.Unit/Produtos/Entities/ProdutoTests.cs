using AwesomeAssertions;
using MinhaApi.Domain.Produtos.Entities;
using Xunit;

namespace MinhaApi.Tests.Unit.Domain.Produtos;

public class ProdutoTests
{
    [Fact]
    public void ConstructorComDadosValidosDeveCriarProdutoCorretamente()
    {
        var produto = new Produto("Teclado Mecânico", 350.00m);

        produto.Nome.Should().Be("Teclado Mecânico");
        produto.Preco.Should().Be(350.00m);
        produto.Ativo.Should().BeTrue();
        produto.Version.Should().Be(0);
        produto.AtualizadoEm.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ConstructorComNomeVazioOuEmBrancoDeveLancarArgumentException(string nomeInvalido)
    {
        var acao = () => new Produto(nomeInvalido, 100m);

        acao.Should().Throw<ArgumentException>()
            .Where(ex => ex.ParamName == "nome");
    }

    [Theory]
    [InlineData("ab")]     // 2 caracteres - abaixo do minimo
    [InlineData("Produto com nome bem longo que ultrapassa exatamente o limite de cem caracteres permitidos no campo nome")] // > 100
    public void ConstructorComNomeForaDoTamanhoPermitidoDeveLancarArgumentException(string nomeInvalido)
    {
        // Cobre o bug corrigido: a checagem antiga usava "&&" em vez de "||" e
        // nunca disparava pra nenhum tamanho de string - esse teste garante que
        // ambas as pontas (muito curto e muito longo) realmente lancam a exception.
        var acao = () => new Produto(nomeInvalido, 100m);

        acao.Should().Throw<ArgumentException>()
            .Where(ex => ex.ParamName == "nome");
    }

    [Fact]
    public void ConstructorComPrecoNegativoDeveLancarArgumentException()
    {
        var acao = () => new Produto("Produto Válido", -10m);

        acao.Should().Throw<ArgumentException>()
            .Where(ex => ex.ParamName == "preco");
    }

    [Fact]
    public void SetNomeComNomeValidoDeveDefinirNomeEAtualizadoEm()
    {
        var produto = new Produto("Nome Original", 100m);

        produto.SetNome("Nome Novo");

        produto.Nome.Should().Be("Nome Novo");
        produto.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void SetNomeComNomeVazioDeveLancarArgumentException()
    {
        var produto = new Produto("Nome Original", 100m);

        var acao = () => produto.SetNome("");

        acao.Should().Throw<ArgumentException>()
            .Where(ex => ex.ParamName == "nome");
    }

    [Fact]
    public void SetPrecoComPrecoValidoDeveDefinirPrecoEAtualizadoEm()
    {
        var produto = new Produto("Produto", 100m);

        produto.SetPreco(200m);

        produto.Preco.Should().Be(200m);
        produto.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void SetPrecoComPrecoNegativoDeveLancarArgumentException()
    {
        var produto = new Produto("Produto", 100m);

        var acao = () => produto.SetPreco(-1m);

        acao.Should().Throw<ArgumentException>()
            .Where(ex => ex.ParamName == "preco");
    }

    [Fact]
    public void DesativarDeveMarcarComoInativoEDefinirAtualizadoEm()
    {
        var produto = new Produto("Produto", 100m);

        produto.Desativar();

        produto.Ativo.Should().BeFalse();
        produto.AtualizadoEm.Should().NotBeNull();
    }
}