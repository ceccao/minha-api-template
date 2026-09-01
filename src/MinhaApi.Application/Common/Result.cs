namespace MinhaApi.Application.Common;

// Use com parcimonia (README §5.1, PLANO §8.2): so para fluxos de negocio com
// MULTIPLOS caminhos que exigem decisao do chamador (retry, fallback, compensacao).
// Exemplo real de uso: ProdutoService.AtualizarPrecoComRetryAsync - tenta a
// atualizacao, e se bater em conflito de concorrencia otimista, decide sozinho se
// tenta de novo ou desiste, devolvendo um resultado definitivo pro chamador (nem
// exception, nem sucesso "fingido") em vez de propagar erro tecnico pra fora.
public class Result<T>
{
    public bool Sucesso { get; }
    public T? Valor { get; }
    public string? Erro { get; }

    internal Result(bool sucesso, T? valor, string? erro)
    {
        Sucesso = sucesso;
        Valor = valor;
        Erro = erro;
    }
}

// Classe nao-generica separada de proposito (corrige CA1000): permite Result.Ok(valor)
// com o tipo inferido pelo argumento, em vez de forcar Result<ProdutoResponse>.Ok(valor).
// Excecao: Falha<T>(string) nao tem argumento do tipo T pra inferir - nesse caso
// e preciso escrever o tipo explicitamente: Result.Falha<ProdutoResponse>("erro").
public static class Result
{
    public static Result<T> Ok<T>(T valor) => new(true, valor, null);
    public static Result<T> Falha<T>(string erro) => new(false, default, erro);
}
