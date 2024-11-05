using ChatRoom.Domain.Models;
using ChatRoom.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace ChatRoom.Messages.Common.HttpHandlers;

public sealed class AuthHandler : DelegatingHandler
{
    private readonly ITokenProviderApiClient _tokenProviderApiClient;
    private TokenResponse? _tokenResponse;
    private readonly ILogger<AuthHandler> _logger;
    private ConsumerUser _consumerUser;

    public AuthHandler(ITokenProviderApiClient tokenProviderApiClient,
                       ILogger<AuthHandler> logger,
                       IOptions<ConsumerUser> options)
    {
        _tokenProviderApiClient = tokenProviderApiClient;
        _logger = logger;
        _consumerUser = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_tokenResponse == null || IsTokenExpired(_tokenResponse))
        {
            try
            {
                var email = _consumerUser.Email;
                var password = _consumerUser.Password;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    throw new InvalidOperationException("User credentials are not properly configured in appsettings.");
                }

                _tokenResponse = await _tokenProviderApiClient.LoginAsync(email, password, cancellationToken);
                _logger.LogInformation("Access token obtained successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to obtain or refresh token.");
                throw;
            }
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenResponse.AccessToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private static bool IsTokenExpired(TokenResponse token)
    {
        return DateTime.UtcNow > token.IssuedAt.AddSeconds(token.ExpiresIn);
    }
}