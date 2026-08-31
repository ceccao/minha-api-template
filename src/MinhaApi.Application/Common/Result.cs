namespace MinhaApi.Application.Common;

// Use com parcimonia (README §5.1, PLANO §8.2): so para fluxos de negocio com
// MULTIPLOS caminhos que exigem decisao do chamador (retry, fallback, compensacao).
// Erro que so precisa parar e responder (404, 400, 409) usa exception tipada
// (MinhaApi.CrossCutting.Exceptions), nao isto - ver ProdutoService para exemplo
// do caso comum (exception), que e a maioria dos casos no template.
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

// Classe nao-generica separada de proposito (corrige CA1000 - "Do not declare static
// members on generic types"): permite Result.Ok(valor) com o tipo inferido pelo
// argumento, em vez de forcar Result<ProdutoResponse>.Ok(valor) toda vez.
// Excecao: Falha<T>(string) nao tem argumento do tipo T pra inferir - nesse caso
// especifico e preciso escrever o tipo explicitamente: Result.Falha<ProdutoResponse>("erro").
public static class Result
{
    public static Result<T> Ok<T>(T valor) => new(true, valor, null);
    public static Result<T> Falha<T>(string erro) => new(false, default, erro);
}