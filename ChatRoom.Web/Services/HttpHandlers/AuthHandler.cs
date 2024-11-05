using ChatRoom.Domain.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChatRoom.Web.Services.HttpHandlers;

internal sealed class AuthHandler : DelegatingHandler
{
    private readonly ITokenProviderApiClient _tokenProviderApiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISessionTokenService _sessionTokenService;

    public AuthHandler(ITokenProviderApiClient tokenProviderApiClient, IHttpContextAccessor httpContextAccessor, ISessionTokenService sessionTokenService)
    {
        _tokenProviderApiClient = tokenProviderApiClient;
        _httpContextAccessor = httpContextAccessor;
        _sessionTokenService = sessionTokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenResponse = _sessionTokenService.GetTokenResponse();

        if (IsTokenExpired(tokenResponse))
        {
            try
            {
                tokenResponse = await _tokenProviderApiClient.RefreshTokenAsync(tokenResponse, cancellationToken);
                var updatedTokenJson = JsonSerializer.Serialize(tokenResponse);
                _httpContextAccessor.HttpContext!.Session.SetString(nameof(TokenResponse), updatedTokenJson);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Failed to refresh token.", ex);
            }
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private static bool IsTokenExpired(TokenResponse token)
    {
        return DateTime.UtcNow > token.IssuedAt.AddSeconds(token.ExpiresIn);
    }
}