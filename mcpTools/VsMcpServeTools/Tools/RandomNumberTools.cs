using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using VsMcpServeTools.Services;

/// <summary>
/// Sample MCP tools for demonstration purposes.
/// These tools can be invoked by MCP clients to perform various operations.
/// </summary>
internal class RandomNumberTools
{
    private readonly AuthenticationService _authenticationService;
    private readonly ILogger<RandomNumberTools> _logger;

    public RandomNumberTools(AuthenticationService authenticationService, ILogger<RandomNumberTools> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    [McpServerTool]
    [Description("Generates a random number between the specified minimum and maximum values.")]
    public int GetRandomNumber(
        [Description("Minimum value (inclusive)")] int min = 0,
        [Description("Maximum value (exclusive)")] int max = 100,
        [Description("API key for authentication (optional if not configured)")] string? apiKey = null)
    {
        if (!_authenticationService.ValidateApiKey(apiKey))
        {
            _logger.LogWarning("Unauthorized access attempt to GetRandomNumber");
            throw new UnauthorizedAccessException("Invalid or missing API key. Please provide a valid API key.");
        }

        _logger.LogInformation("Generating random number between {Min} and {Max}", min, max);
        return Random.Shared.Next(min, max);
    }

    [McpServerTool]
    [Description("Get Uppis/Upendra details/description.")]
    public string GetUppiDetails(
        [Description("API key for authentication (optional if not configured)")] string? apiKey = null)
    {
        if (!_authenticationService.ValidateApiKey(apiKey))
        {
            _logger.LogWarning("Unauthorized access attempt to GetUppiDetails");
            throw new UnauthorizedAccessException("Invalid or missing API key. Please provide a valid API key.");
        }

        _logger.LogInformation("Retrieving Uppi details");
        return "Uppi is a kannada hero, who is versatile actor, his wife is Priyanka";
    }
}
