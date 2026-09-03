using MinhaApi.Domain.Abstractions;

namespace MinhaApi.Application.Produtos.DataTransfer.Requests;

// Substitui o antigo PaginacaoRequest (so pagina/tamanho) e o ProdutoFilter (so
// filtros) - agora e um objeto so por entidade, com ordenacao padrao fixada no
// construtor (o front so troca TpOrd ao clicar numa coluna, nao manda qualquer
// nome de campo livre).
public class ListarProdutosRequest : PaginacaoFiltro
{
    public string? Nome { get; set; }
    public decimal? PrecoMinimo { get; set; }
    public decimal? PrecoMaximo { get; set; }
    public bool? Ativo { get; set; }

    public ListarProdutosRequest() : base(cpOrd: "Id", tpOrd: TipoOrdenacao.Ascendente)
    {
    }
}