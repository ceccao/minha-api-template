namespace MinhaApi.Application.Common;

public record PagedResult<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    long TotalItens)
{
    public int TotalPaginas => (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);
}
