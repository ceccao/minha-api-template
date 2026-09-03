namespace MinhaApi.Domain.Abstractions;

// Resultado paginado no nivel de Domain/Infra - distinto do PagedResult<T> que
// existe na Application (esse aqui carrega entidades de dominio, o outro carrega
// DTOs de resposta). Domain/Infra nao podem depender de Application (regra de
// dependencia, README §2.1), entao precisa do proprio tipo aqui.
public class PaginacaoConsulta<TEntidade>(IReadOnlyList<TEntidade> itens, int totalItens, int pagina, int tamanhoPagina)
{
    public IReadOnlyList<TEntidade> Itens { get; } = itens;
    public int TotalItens { get; } = totalItens;
    public int Pagina { get; } = pagina;
    public int TamanhoPagina { get; } = tamanhoPagina;
}
