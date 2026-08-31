namespace MinhaApi.CrossCutting.Exceptions;

public abstract class AppException(string mensagem) : Exception(mensagem)
{
    public abstract int StatusCode { get; }
}
