using MinhaApi.Domain.Abstractions;
using NHibernate;

namespace MinhaApi.Infra.Config;

public class UnitOfWork(ISession session) : IUnitOfWork
{
    private ITransaction? _transacao;

    public void BeginTransaction()
    {
        _transacao = session.BeginTransaction();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transacao is null)
        {
            return;
        }

        await _transacao.CommitAsync(cancellationToken);
        _transacao = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transacao is null)
        {
            return;
        }

        await _transacao.RollbackAsync(cancellationToken);
        _transacao = null;
    }
}