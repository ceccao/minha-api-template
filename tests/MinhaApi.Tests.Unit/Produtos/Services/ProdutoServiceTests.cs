using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using MinhaApi.Application.Produtos.DataTransfer.Requests;
using MinhaApi.Application.Produtos.Services;
using MinhaApi.CrossCutting.Exceptions;
using MinhaApi.Domain.Produtos.Commands;
using MinhaApi.Domain.Produtos.Entities;
using MinhaApi.Domain.Produtos.Repositories;
using MinhaApi.Domain.Produtos.Services.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace MinhaApi.Tests.Unit.Produtos.Services;

public class ProdutoServiceTests
{
    private readonly IProdutosService _produtosService = Substitute.For<IProdutosService>();
    private readonly IProdutoRepository _repository = Substitute.For<IProdutoRepository>();
    private readonly IValidator<CriarProdutoRequest> _criarValidator = Substitute.For<IValidator<CriarProdutoRequest>>();
    private readonly IValidator<AtualizarProdutoRequest> _atualizarValidator = Substitute.For<IValidator<AtualizarProdutoRequest>>();
    private readonly ProdutoService _sut;

    public ProdutoServiceTests()
    {
        _sut = new ProdutoService(_produtosService, _repository, _criarValidator, _atualizarValidator);

        _criarValidator.ValidateAsync(Arg.Any<CriarProdutoRequest>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        _atualizarValidator.ValidateAsync(Arg.Any<AtualizarProdutoRequest>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
    }

    [Fact]
    public async Task CriarAsyncComRequestValidoDeveChamarDomainServiceERetornarResponse()
    {
        var request = new CriarProdutoRequest { Nome = "Teclado", Preco = 350m };
        var produtoCriado = new Produto("Teclado", 350m);
        _produtosService.CriarAsync(Arg.Any<InserirProdutoCommand>(), Arg.Any<CancellationToken>())
            .Returns(produtoCriado);

        var resultado = await _sut.CriarAsync(request, CancellationToken.None);

        resultado.Nome.Should().Be("Teclado");
        resultado.Preco.Should().Be(350m);
        await _produtosService.Received(1).CriarAsync(
            Arg.Is<InserirProdutoCommand>(c => c.Nome == "Teclado" && c.Preco == 350m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CriarAsyncComRequestInvalidoDeveLancarEntidadeInvalidaExceptionSemChamarDomainService()
    {
        var request = new CriarProdutoRequest { Nome = "", Preco = 350m };
        var erros = new List<ValidationFailure> { new("Nome", "Nome é obrigatório.") };
        _criarValidator.ValidateAsync(request, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(erros));

        var acao = async () => await _sut.CriarAsync(request, CancellationToken.None);

        await acao.Should().ThrowAsync<EntidadeInvalidaException>();
        await _produtosService.DidNotReceive().CriarAsync(Arg.Any<InserirProdutoCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecuperarAsyncComIdExistenteDeveRetornarResponse()
    {
        var produto = new Produto("Mouse", 120m);
        _produtosService.RecuperarAsync(1, Arg.Any<CancellationToken>()).Returns(produto);

        var resultado = await _sut.RecuperarAsync(1, CancellationToken.None);

        resultado.Nome.Should().Be("Mouse");
    }

    [Fact]
    public async Task RecuperarAsyncComIdInexistenteDeveLancarNaoEncontradoException()
    {
        _produtosService.RecuperarAsync(99, Arg.Any<CancellationToken>()).Returns((Produto?)null);

        var acao = async () => await _sut.RecuperarAsync(99, CancellationToken.None);

        await acao.Should().ThrowAsync<NaoEncontradoException<Produto>>();
    }

    [Fact]
    public async Task EditarAsyncComProdutoExistenteDeveRetornarResponseAtualizado()
    {
        var request = new AtualizarProdutoRequest { Nome = "Nome Novo", Preco = 200m };
        var produtoEditado = new Produto("Nome Novo", 200m);
        _produtosService.EditarAsync(1, Arg.Any<EditarProdutoCommand>(), Arg.Any<CancellationToken>())
            .Returns(produtoEditado);

        var resultado = await _sut.EditarAsync(1, request, CancellationToken.None);

        resultado.Nome.Should().Be("Nome Novo");
        resultado.Preco.Should().Be(200m);
        await _produtosService.Received(1).EditarAsync(
            1,
            Arg.Is<EditarProdutoCommand>(c => c.Nome == "Nome Novo" && c.Preco == 200m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EditarAsyncComProdutoInexistenteDeveLancarNaoEncontradoException()
    {
        var request = new AtualizarProdutoRequest { Nome = "Nome", Preco = 100m };
        _produtosService.EditarAsync(99, Arg.Any<EditarProdutoCommand>(), Arg.Any<CancellationToken>())
            .Returns((Produto?)null);

        var acao = async () => await _sut.EditarAsync(99, request, CancellationToken.None);

        await acao.Should().ThrowAsync<NaoEncontradoException<Produto>>();
    }

    [Fact]
    public async Task ExcluirAsyncComProdutoExistenteNaoDeveLancarExcecao()
    {
        _produtosService.ExcluirAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var acao = async () => await _sut.ExcluirAsync(1, CancellationToken.None);

        await acao.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExcluirAsyncComProdutoInexistenteDeveLancarNaoEncontradoException()
    {
        _produtosService.ExcluirAsync(99, Arg.Any<CancellationToken>()).Returns(false);

        var acao = async () => await _sut.ExcluirAsync(99, CancellationToken.None);

        await acao.Should().ThrowAsync<NaoEncontradoException<Produto>>();
    }

    [Fact]
    public async Task AtualizarPrecoComRetryAsyncSemConflitoDeveRetornarResultDeSucesso()
    {
        var produto = new Produto("Produto", 100m);
        _repository.RecuperarAsync(1, Arg.Any<CancellationToken>()).Returns(produto);

        var resultado = await _sut.AtualizarPrecoComRetryAsync(1, 150m);

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Preco.Should().Be(150m);
    }

    [Fact]
    public async Task AtualizarPrecoComRetryAsyncComConflitoPersistenteDeveRetornarFalhaAposEsgotarTentativas()
    {
        var produto = new Produto("Produto", 100m);
        _repository.RecuperarAsync(1, Arg.Any<CancellationToken>()).Returns(produto);
        _repository.EditarAsync(Arg.Any<Produto>(), Arg.Any<CancellationToken>())
            .Throws(new ConflitoException("Conflito simulado."));

        var resultado = await _sut.AtualizarPrecoComRetryAsync(1, 150m, maxTentativas: 2);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("2 tentativas");
        await _repository.Received(2).EditarAsync(Arg.Any<Produto>(), Arg.Any<CancellationToken>());
    }
}