namespace MinhaApi.Application.Common;

// Classe (nao record) de proposito: precisa de propriedades com "set" para o model
// binding da query string (?pagina=2&tamanhoPagina=50) funcionar corretamente.
public class PaginacaoRequest
{
    private const int TamanhoPaginaPadrao = 20;
    private const int TamanhoPaginaMaximo = 100; // limite de seguranca (README §6.2 - payload malicioso/DoS)

    private int _pagina = 1;
    private int _tamanhoPagina = TamanhoPaginaPadrao;

    public int Pagina
    {
        get => _pagina;
        set => _pagina = value < 1 ? 1 : value;
    }

    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set => _tamanhoPagina = value switch
        {
            < 1 => TamanhoPaginaPadrao,
            > TamanhoPaginaMaximo => TamanhoPaginaMaximo,
            _ => value
        };
    }
}
