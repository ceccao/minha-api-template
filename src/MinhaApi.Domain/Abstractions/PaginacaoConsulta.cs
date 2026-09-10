namespace MinhaApi.Domain.Abstractions;

public class PaginacaoConsulta<TEntidade>(IReadOnlyList<TEntidade> itens, int totalItens, int pagina, int tamanhoPagina)
{
    public IReadOnlyList<TEntidade> Itens { get; } = itens;
    public int TotalItens { get; } = totalItens;
    public int Pagina { get; } = pagina;
    public int TamanhoPagina { get; } = tamanhoPagina;
}
