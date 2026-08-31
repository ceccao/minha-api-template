using System.Globalization;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using MinhaApi.Application.Abstractions;

namespace MinhaApi.Infra.Services;

// Implementacao basica via SmtpClient (biblioteca padrao do .NET, sem pacote extra).
// Configuracao real (host, porta, credenciais) vem de User Secrets/variavel de
// ambiente na secao "Smtp" - nunca hardcoded (README §6.2).
public class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task EnviarAsync(string destinatario, string assunto, string corpo, CancellationToken cancellationToken = default)
    {
        var host = configuration["Smtp:Host"]
            ?? throw new InvalidOperationException("Smtp:Host nao configurado.");
        var port = int.Parse(configuration["Smtp:Port"] ?? "587", CultureInfo.InvariantCulture);
        var usuario = configuration["Smtp:Usuario"];
        var senha = configuration["Smtp:Senha"];
        var remetente = configuration["Smtp:Remetente"]
            ?? usuario
            ?? throw new InvalidOperationException("Smtp:Remetente nao configurado.");

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(usuario, senha)
        };

        using var mensagem = new MailMessage(remetente, destinatario, assunto, corpo);

        await client.SendMailAsync(mensagem, cancellationToken);
    }
}