namespace MinhaApi.Domain.Produtos.Repositories.Filters;

// Objeto de criterios simples para consultas com multiplos parametros opcionais.
// NAO e o Specification Pattern completo (esse continua adiado - README §5.4).
public class ProdutoFilter
{
    public string? Nome { get; set; }
    public decimal? PrecoMinimo { get; set; }
    public decimal? PrecoMaximo { get; set; }
    public bool? Ativo { get; set; }
}
