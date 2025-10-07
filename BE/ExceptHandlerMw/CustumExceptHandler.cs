using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json; // Use System.Text.Json for serialization
using System.Threading;
using System.Threading.Tasks;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the exception
        _logger.LogError(
            exception, "An unhandled exception occurred: {ErrorMessage}", exception.Message);

        // Set the response status code
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Set the response content type
        httpContext.Response.ContentType = "application/json";

        // Create a structured error response
        // In a production environment, you would typically provide a more generic message
        // and avoid exposing specific exception details to the client.
        var problemDetails = new
        {
            Status = httpContext.Response.StatusCode,
            Title = "An error occurred while processing your request.",
            Detail = _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
            Instance = httpContext.Request.Path
        };

        // Serialize and write the response
        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions { WriteIndented = true }); // Make it readable
        await httpContext.Response.WriteAsync(json, cancellationToken);

        // Return true to indicate that the exception has been handled
        return true;
    }
}