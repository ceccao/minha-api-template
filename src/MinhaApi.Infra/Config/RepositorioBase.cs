using MinhaApi.CrossCutting.Exceptions;
using MinhaApi.Domain.Abstractions;
using NHibernate;
using NHibernate.Linq;

namespace MinhaApi.Infra.Config;

public class RepositorioBase<TEntidade>(ISession session) : IRepositorioBase<TEntidade>
    where TEntidade : class
{
    protected ISession Session { get; } = session;

    public virtual async Task<TEntidade?> RecuperarAsync(long id, CancellationToken cancellationToken = default)
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
        try
        {
            await Session.UpdateAsync(entidade, cancellationToken);

            // Flush explicito de proposito: forca o conflito de Version a estourar AQUI,
            // dentro do Await do Service, em vez de silenciosamente no fim do request.
            await Session.FlushAsync(cancellationToken);
        }
        catch (StaleObjectStateException)
        {
            // Traduz o tipo especifico do NHibernate pro nosso proprio tipo (README §7.3).
            // Isso mantem o CrossCutting/Application livres de conhecer o NHibernate -
            // só a Infra (que já depende do ORM mesmo) sabe que esse erro existe.
            throw new ConflitoException("O recurso foi modificado por outra operação. Recarregue e tente novamente.");
        }
    }

    public virtual async Task ExcluirAsync(TEntidade entidade, CancellationToken cancellationToken = default)
        => await Session.DeleteAsync(entidade, cancellationToken);

    public virtual async Task ExcluirPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entidade = await RecuperarAsync(id, cancellationToken);
        if (entidade is not null)
        {
            await ExcluirAsync(entidade, cancellationToken);
        }
    }
}
