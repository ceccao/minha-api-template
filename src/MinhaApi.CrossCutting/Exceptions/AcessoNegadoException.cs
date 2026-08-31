using Microsoft.AspNetCore.Http;

namespace MinhaApi.CrossCutting.Exceptions;

// Peca usada na defesa contra IDOR (README §6.3): quando a Application valida posse
// do recurso (UsuarioLogadoId/TenantId) e o usuario autenticado nao tem acesso aquele
// recurso especifico, lanca esta exception em vez de um generico NaoEncontradoException
// (nao "vaza" pro cliente se o recurso existe ou nao, so nega o acesso).
public class AcessoNegadoException(string mensagem = "Acesso negado ao recurso.")
    : AppException(mensagem)
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
}
