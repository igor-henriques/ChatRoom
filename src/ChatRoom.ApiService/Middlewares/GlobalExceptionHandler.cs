using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Text.Json;

namespace ChatRoom.ApiService.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private const string InternalServerLogErrorMessage = "Internal server error: {Exception}";
    private const string DomainLogErrorMessage = "Domain error: {Exception}";

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException)
        {
            _logger.LogError(InternalServerLogErrorMessage, exception);
        }
        else
        {
            _logger.LogWarning(DomainLogErrorMessage, exception);
        }

        (int statusCode, string details) = exception switch
        {
            JsonException or { InnerException: JsonException } or BadHttpRequestException => (StatusCodes.Status400BadRequest, "Invalid request"),
            ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, exception.Message),
            var ex when ex is DbUpdateException && ex.InnerException != null && ex.InnerException.Message.Contains("duplicate") => (StatusCodes.Status400BadRequest, "Duplicates are not allowed"),
            InvalidCredentialException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An internal error occurred. Try again later.")
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetail = new ProblemDetails
        {
            Status = statusCode,
            Title = "Something went wrong.",
            Detail = details
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetail, cancellationToken);

        return true;
    }
}
