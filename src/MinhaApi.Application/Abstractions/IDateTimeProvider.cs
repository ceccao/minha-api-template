namespace MinhaApi.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

// Implementacao trivial o suficiente pra ficar junto da interface (sem I/O externo).
// Em testes, mocka-se IDateTimeProvider pra fixar um horario conhecido em vez de
// depender do relogio real da maquina.
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
