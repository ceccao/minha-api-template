using Microsoft.AspNetCore.Http;

namespace MinhaApi.CrossCutting.Exceptions;

public class NaoEncontradoException<T>(object id)
    : AppException($"{typeof(T).Name} com id '{id}' não foi encontrado.")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
}
