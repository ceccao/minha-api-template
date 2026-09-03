using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using MinhaApi.Application.Produtos.Services;
using MinhaApi.CrossCutting.Exceptions;
using MinhaApi.Domain.Produtos.Commands;
using MinhaApi.Domain.Produtos.Entities;
using MinhaApi.Domain.Produtos.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace MinhaApi.Tests.Unit.Produtos.Services;

public class ProdutoServiceTests
{
    private readonly IProdutoRepository _repository = Substitute.For<IProdutoRepository>();
    private readonly IValidator<CriarProdutoCommand> _criarValidator = Substitute.For<IValidator<CriarProdutoCommand>>();
    private readonly IValidator<AtualizarProdutoCommand> _atualizarValidator = Substitute.For<IValidator<AtualizarProdutoCommand>>();
    private readonly ProdutoService _sut;

    public ProdutoServiceTests()
    {
        _sut = new ProdutoService(_repository, _criarValidator, _atualizarValidator);

        _criarValidator.ValidateAsync(Arg.Any<CriarProdutoCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        _atualizarValidator.ValidateAsync(Arg.Any<AtualizarProdutoCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
    }

    [Fact]
    public async Task CriarAsyncComCommandValidoDeveInserirERetornarResponseCorreto()
    {
        var command = new CriarProdutoCommand("Teclado", 350m);

        var resultado = await _sut.CriarAsync(command);

        resultado.Nome.Should().Be("Teclado");
        resultado.Preco.Should().Be(350m);
        await _repository.Received(1).InserirAsync(Arg.Any<Produto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CriarAsyncComCommandInvalidoDeveLancarEntidadeInvalidaExceptionSemInserir()
    {
        var command = new CriarProdutoCommand("", 350m);
        var erros = new List<ValidationFailure> { new("Nome", "Nome é obrigatório.") };
        _criarValidator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(erros));

        var acao = async () => await _sut.CriarAsync(command);

        await acao.Should().ThrowAsync<EntidadeInvalidaException>();
        await _repository.DidNotReceive().InserirAsync(Arg.Any<Produto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ObterPorIdAsyncComIdExistenteDeveRetornarResponse()
    {
        var produto = new Produto("Mouse", 120m);
        _repository.RecuperarAsync(1, Arg.Any<CancellationToken>()).Returns(produto);

        var resultado = await _sut.ObterPorIdAsync(1);

        resultado.Nome.Should().Be("Mouse");
    }

    [Fact]
    public async Task ObterPorIdAsyncComIdInexistenteDeveLancarNaoEncontradoException()
    {
        _repository.RecuperarAsync(99, Arg.Any<CancellationToken>()).Returns((Produto?)null);

        var acao = async () => await _sut.ObterPorIdAsync(99);

        await acao.Should().ThrowAsync<NaoEncontradoException<Produto>>();
    }

    [Fact]
    public async Task AtualizarAsyncComProdutoExistenteDeveAtualizarNomeEPreco()
    {
        var produto = new Produto("Nome Antigo", 100m);
        var command = new AtualizarProdutoCommand(1, "Nome Novo", 200m);
        _repository.RecuperarAsync(1, Arg.Any<CancellationToken>()).Returns(produto);

        var resultado = await _sut.AtualizarAsync(command);

        resultado.Nome.Should().Be("Nome Novo");
        resultado.Preco.Should().Be(200m);
        await _repository.Received(1).EditarAsync(produto, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AtualizarAsyncComProdutoInexistenteDeveLancarNaoEncontradoException()
    {
        var command = new AtualizarProdutoCommand(99, "Nome", 100m);
        _repository.RecuperarAsync(99, Arg.Any<CancellationToken>()).Returns((Produto?)null);

        var acao = async () => await _sut.AtualizarAsync(command);

        await acao.Should().ThrowAsync<NaoEncontradoException<Produto>>();
    }

    [Fact]
    public async Task ExcluirAsyncComProdutoExistenteDeveDesativarEChamarEditarAsync()
    {
        var produto = new Produto("Produto", 100m);
        _repository.RecuperarAsync(1, Arg.Any<CancellationToken>()).Returns(produto);

        await _sut.ExcluirAsync(1);

        produto.Ativo.Should().BeFalse();
        await _repository.Received(1).EditarAsync(produto, Arg.Any<CancellationToken>());
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