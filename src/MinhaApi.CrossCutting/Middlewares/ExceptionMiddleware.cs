using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinhaApi.CrossCutting.Exceptions;
using NHibernate;

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
        var statusCode = exception switch
        {
            AppException appException => appException.StatusCode,
            StaleObjectStateException => StatusCodes.Status409Conflict, // concorrencia otimista, README §7.3
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
            Title = exception is StaleObjectStateException
                ? "O recurso foi modificado por outra operação. Recarregue e tente novamente."
                : exception.Message,
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