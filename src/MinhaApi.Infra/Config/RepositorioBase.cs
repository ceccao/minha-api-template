using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using MinhaApi.CrossCutting.Exceptions;
using MinhaApi.Domain.Abstractions;
using NHibernate;
using NHibernate.Linq;

namespace MinhaApi.Infra.Config;

public class RepositorioBase<TEntidade>(ISession session) : IRepositorioBase<TEntidade>
    where TEntidade : class
{
    protected ISession Session { get; } = session;

    public virtual async Task InserirAsync(TEntidade entidade, CancellationToken cancellationToken = default)
        => await Session.SaveAsync(entidade, cancellationToken);

    public virtual async Task InserirAsync(IEnumerable<TEntidade> entidades, CancellationToken cancellationToken = default)
    {
        foreach (var entidade in entidades)
        {
            await Session.SaveAsync(entidade, cancellationToken);
        }
    }

    public virtual async Task EditarAsync(TEntidade entidade, CancellationToken cancellationToken = default)
    {
        try
        {
            await Session.UpdateAsync(entidade, cancellationToken);
            await Session.FlushAsync(cancellationToken);
        }
        catch (StaleObjectStateException)
        {
            throw new ConflitoException("O recurso foi modificado por outra operação. Recarregue e tente novamente.");
        }
    }

    public virtual async Task ExcluirAsync(TEntidade entidade, CancellationToken cancellationToken = default)
        => await Session.DeleteAsync(entidade, cancellationToken);

    public virtual async Task ExcluirAsync(IEnumerable<TEntidade> entidades, CancellationToken cancellationToken = default)
    {
        foreach (var entidade in entidades)
        {
            await Session.DeleteAsync(entidade, cancellationToken);
        }
    }

    public virtual async Task<TEntidade?> RecuperarAsync(int id, CancellationToken cancellationToken = default)
        => await Session.GetAsync<TEntidade>(id, cancellationToken);

    public virtual async Task<TEntidade?> RecuperarAsync(Expression<Func<TEntidade, bool>> expressao, CancellationToken cancellationToken = default)
        => await Session.Query<TEntidade>().Where(expressao).FirstOrDefaultAsync(cancellationToken);

    public virtual Task<PaginacaoConsulta<TEntidade>> ListarAsync(
        PaginacaoFiltro paginacao,
        Expression<Func<TEntidade, bool>>? filtro = null,
        CancellationToken cancellationToken = default)
        => ListarInternoAsync(
            paginacao.Qt, paginacao.Pg, filtro,
            query => string.IsNullOrWhiteSpace(paginacao.CpOrd)
                ? query
                : query.OrderBy(ClausulaOrdenacao(paginacao.CpOrd, paginacao.TpOrd)),
            cancellationToken);

    public virtual Task<PaginacaoConsulta<TEntidade>> ListarAsync(
        int qt,
        int pg,
        (string Campo, TipoOrdenacao Tipo)[] ordenacao,
        Expression<Func<TEntidade, bool>>? filtro = null,
        CancellationToken cancellationToken = default)
        => ListarInternoAsync(
            qt, pg, filtro,
            query => ordenacao.Length == 0 ? query : query.OrderBy(ClausulaOrdenacao(ordenacao)),
            cancellationToken);

    private async Task<PaginacaoConsulta<TEntidade>> ListarInternoAsync(
        int qt,
        int pg,
        Expression<Func<TEntidade, bool>>? filtro,
        Func<IQueryable<TEntidade>, IQueryable<TEntidade>> aplicarOrdenacao,
        CancellationToken cancellationToken)
    {
        var query = Session.Query<TEntidade>();

        if (filtro is not null)
        {
            query = query.Where(filtro);
        }

        var totalItens = await query.CountAsync(cancellationToken);

        query = aplicarOrdenacao(query);

        var itens = await query
            .Skip((pg - 1) * qt)
            .Take(qt)
            .ToListAsync(cancellationToken);

        return new PaginacaoConsulta<TEntidade>(itens, totalItens, pg, qt);
    }

    // Monta a clausula no formato exigido pelo System.Linq.Dynamic.Core (ex: "Preco
    // descending"). Diferente de SQL cru: o Dynamic.Core valida "Preco" como uma
    // propriedade real de TEntidade via reflection antes de montar a expressao -
    // um nome invalido lanca ParseException, nunca executa nada no banco.
    private static string ClausulaOrdenacao(string campo, TipoOrdenacao tipo)
        => $"{campo} {(tipo == TipoOrdenacao.Descendente ? "descending" : "ascending")}";

    private static string ClausulaOrdenacao((string Campo, TipoOrdenacao Tipo)[] ordenacao)
        => string.Join(", ", ordenacao.Select(o => ClausulaOrdenacao(o.Campo, o.Tipo)));
}