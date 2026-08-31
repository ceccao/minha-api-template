using Microsoft.AspNetCore.Http;

namespace MinhaApi.CrossCutting.Exceptions;

public class ConflitoException(string mensagem) : AppException(mensagem)
{
    public override int StatusCode => StatusCodes.Status409Conflict;
}
