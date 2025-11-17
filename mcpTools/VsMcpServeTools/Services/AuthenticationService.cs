using System;

namespace VsMcpServeTools.Services;

/// <summary>
/// Service for handling authentication of MCP tool requests.
/// </summary>
public class AuthenticationService
{
    private const string ApiKeyEnvironmentVariable = "VSMCP_API_KEY";
    private readonly string? _configuredApiKey;
    private readonly bool _authenticationEnabled;

    public AuthenticationService()
    {
        _configuredApiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
        _authenticationEnabled = !string.IsNullOrWhiteSpace(_configuredApiKey);
    }

    /// <summary>
    /// Validates the provided API key against the configured key.
    /// </summary>
    /// <param name="apiKey">The API key to validate.</param>
    /// <returns>True if the API key is valid or authentication is disabled, false otherwise.</returns>
    public bool ValidateApiKey(string? apiKey)
    {
        // If authentication is not enabled (no API key configured), allow all requests
        if (!_authenticationEnabled)
        {
            return true;
        }

        // If authentication is enabled, validate the provided API key
        return !string.IsNullOrWhiteSpace(apiKey) && 
               apiKey.Equals(_configuredApiKey, StringComparison.Ordinal);
    }

    /// <summary>
    /// Gets whether authentication is currently enabled.
    /// </summary>
    public bool IsAuthenticationEnabled => _authenticationEnabled;
}
