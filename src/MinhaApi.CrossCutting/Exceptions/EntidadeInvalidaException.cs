using Microsoft.AspNetCore.Http;

namespace MinhaApi.CrossCutting.Exceptions;

public class EntidadeInvalidaException(IEnumerable<string> erros)
    : AppException("Um ou mais erros de validação ocorreram.")
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public IEnumerable<string> Erros { get; } = erros;
}
