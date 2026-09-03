namespace MinhaApi.Domain.Common;

public abstract class EntidadeBase
{
    public virtual int Id { get; protected set; }
    public virtual bool Ativo { get; protected set; } = true;
    public virtual DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
    public virtual DateTime? AtualizadoEm { get; protected set; }
    public virtual int Version { get; protected set; }

    public virtual void Desativar()
    {
        Ativo = false;
        AtualizadoEm = DateTime.UtcNow;
    }
}
