namespace MinhaApi.Domain.Abstractions;

public interface IRepositorioBase<TEntidade> where TEntidade : class
{
    Task<TEntidade?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntidade>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task InserirAsync(TEntidade entidade, CancellationToken cancellationToken = default);
    Task InserirAsync(IEnumerable<TEntidade> entidades, CancellationToken cancellationToken = default);
    Task AtualizarAsync(TEntidade entidade, CancellationToken cancellationToken = default);
    Task ExcluirAsync(TEntidade entidade, CancellationToken cancellationToken = default);
    Task ExcluirPorIdAsync(long id, CancellationToken cancellationToken = default);
}
