namespace MinhaApi.Application.Abstractions;

public interface IEmailSender
{
    Task EnviarAsync(string destinatario, string assunto, string corpo, CancellationToken cancellationToken = default);
}
