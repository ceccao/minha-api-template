using MinhaApi.Domain.Abstractions;
using NHibernate;
using NHibernate.Linq;

namespace MinhaApi.Infra.Repositories;

public class RepositorioBase<TEntidade>(ISession session) : IRepositorioBase<TEntidade>
    where TEntidade : class
{
    protected ISession Session { get; } = session;

    public virtual async Task<TEntidade?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
        => await Session.GetAsync<TEntidade>(id, cancellationToken);

    public virtual async Task<IEnumerable<TEntidade>> ObterTodosAsync(CancellationToken cancellationToken = default)
        => await Session.Query<TEntidade>().ToListAsync(cancellationToken);

    public virtual async Task InserirAsync(TEntidade entidade, CancellationToken cancellationToken = default)
        => await Session.SaveAsync(entidade, cancellationToken);

    public virtual async Task InserirAsync(IEnumerable<TEntidade> entidades, CancellationToken cancellationToken = default)
    {
        foreach (var entidade in entidades)
        {
            await Session.SaveAsync(entidade, cancellationToken);
        }
    }

    public virtual async Task AtualizarAsync(TEntidade entidade, CancellationToken cancellationToken = default)
    {
        await Session.UpdateAsync(entidade, cancellationToken);

        // Flush explicito de proposito: forca o StaleObjectStateException (se houver
        // conflito de Version) a estourar AQUI, dentro do Await do Service, em vez de
        // silenciosamente no fim do request - garante que o ExceptionMiddleware
        // (README §7.3) sempre consiga capturar e traduzir pra 409.
        await Session.FlushAsync(cancellationToken);
    }

    public virtual async Task ExcluirAsync(TEntidade entidade, CancellationToken cancellationToken = default)
        => await Session.DeleteAsync(entidade, cancellationToken);

    public virtual async Task ExcluirPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entidade = await ObterPorIdAsync(id, cancellationToken);
        if (entidade is not null)
        {
            await ExcluirAsync(entidade, cancellationToken);
        }
    }
}