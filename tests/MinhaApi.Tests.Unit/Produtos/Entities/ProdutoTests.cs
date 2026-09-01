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

    [Fact]
    public void ConstructorComPrecoNegativoDeveLancarArgumentException()
    {
        var acao = () => new Produto("Produto Válido", -10m);

        acao.Should().Throw<ArgumentException>()
            .Where(ex => ex.ParamName == "preco");
    }

    [Fact]
    public void AtualizarNomeComNomeValidoDeveAtualizarNomeEDefinirAtualizadoEm()
    {
        var produto = new Produto("Nome Original", 100m);

        produto.AtualizarNome("Nome Novo");

        produto.Nome.Should().Be("Nome Novo");
        produto.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void AtualizarNomeComNomeVazioDeveLancarArgumentException()
    {
        var produto = new Produto("Nome Original", 100m);

        var acao = () => produto.AtualizarNome("");

        acao.Should().Throw<ArgumentException>()
            .Where(ex => ex.ParamName == "novoNome");
    }

    [Fact]
    public void AtualizarPrecoComPrecoValidoDeveAtualizarPrecoEDefinirAtualizadoEm()
    {
        var produto = new Produto("Produto", 100m);

        produto.AtualizarPreco(200m);

        produto.Preco.Should().Be(200m);
        produto.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void AtualizarPrecoComPrecoNegativoDeveLancarArgumentException()
    {
        var produto = new Produto("Produto", 100m);

        var acao = () => produto.AtualizarPreco(-1m);

        acao.Should().Throw<ArgumentException>()
            .Where(ex => ex.ParamName == "novoPreco");
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