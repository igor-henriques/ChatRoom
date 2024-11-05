using ChatRoom.Domain.Models;
using ChatRoom.Domain.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ChatRoom.Infrastructure.Services;

public sealed class TokenProviderApiClient : ITokenProviderApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TokenProviderApiClient> _logger;

    public TokenProviderApiClient(HttpClient httpClient, ILogger<TokenProviderApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TokenResponse> LoginAsync(string email,
                                                string password,
                                                CancellationToken cancellationToken = default)
    {
        var loginRequest = new { Email = email, Password = password };
        return await SendRequestAsync("/login", loginRequest, "login", cancellationToken);
    }

    public async Task<TokenResponse> RefreshTokenAsync(TokenResponse token, CancellationToken cancellationToken = default)
    {
        var refreshRequest = new { token.RefreshToken };
        return await SendRequestAsync("/refresh", refreshRequest, "refresh token", cancellationToken);
    }

    private async Task<TokenResponse> SendRequestAsync(string endpoint,
                                                       object requestBody,
                                                       string operation,
                                                       CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, requestBody, cancellationToken);
            return await HandleResponseAsync(response, operation, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the {Operation} process.", operation);
            throw new ApplicationException($"An error occurred during the {operation} process.", ex);
        }
    }

    private async Task<TokenResponse> HandleResponseAsync(HttpResponseMessage response,
                                                          string operation,
                                                          CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("{Operation} failed with status code: {StatusCode}", operation, response.StatusCode);
            throw new HttpRequestException($"{operation} failed with status code: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
        if (result == null)
        {
            _logger.LogError("The response for {Operation} did not contain a valid token.", operation);
            throw new InvalidOperationException($"The response for {operation} did not contain a valid token.");
        }

        _logger.LogInformation("{Operation} successful. Token obtained.", operation);
        return result;
    }
}