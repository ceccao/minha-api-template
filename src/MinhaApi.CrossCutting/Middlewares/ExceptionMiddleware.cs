using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinhaApi.CrossCutting.Exceptions;

namespace MinhaApi.CrossCutting.Middlewares;

public partial class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // StaleObjectStateException do NHibernate (concorrencia otimista) agora e
        // traduzida em ConflitoException direto no RepositorioBase (Infra) - o
        // CrossCutting nao precisa mais conhecer o NHibernate pra tratar isso.
        var statusCode = exception switch
        {
            AppException appException => appException.StatusCode,
            _ => StatusCodes.Status500InternalServerError
        };

        // AppException = erro de negocio esperado (warning, nao polui alerta).
        // Qualquer outra coisa = erro tecnico inesperado (error, aciona alerta real).
        if (exception is AppException)
        {
            LogErroDeNegocio(logger, exception.Message, exception);
        }
        else
        {
            LogErroNaoTratado(logger, exception);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception.Message,
        };
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is EntidadeInvalidaException entidadeInvalida)
        {
            problemDetails.Extensions["errors"] = entidadeInvalida.Erros;
        }

        // Em producao, nunca expor stack trace, nome de tabela ou detalhe interno (README §6.3).
        // AppException.Message ja e uma mensagem pensada pra ser publica, entao fica de fora dessa checagem.
        if (environment.IsDevelopment() && exception is not AppException)
        {
            problemDetails.Detail = exception.ToString();
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Erro de negocio tratado: {Message}")]
    private static partial void LogErroDeNegocio(ILogger logger, string message, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Erro nao tratado")]
    private static partial void LogErroNaoTratado(ILogger logger, Exception exception);
}
