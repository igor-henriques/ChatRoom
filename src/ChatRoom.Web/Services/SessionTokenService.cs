using ChatRoom.Domain.Models;
using System.Text.Json;

namespace ChatRoom.Web.Services;

public sealed class SessionTokenService : ISessionTokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionTokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public TokenResponse GetTokenResponse()
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HTTP context is not available.");

        var serializedTokenResponse = httpContext.Session.GetString(nameof(TokenResponse));
        if (string.IsNullOrEmpty(serializedTokenResponse))
        {
            throw new UnauthorizedAccessException("No access token found in session.");
        }

        try
        {
            TokenResponse tokenResponse = JsonSerializer.Deserialize<TokenResponse>(serializedTokenResponse)
                            ?? throw new InvalidOperationException("Failed to deserialize TokenResponse.");

            return tokenResponse;
        }
        catch (JsonException ex)
        {
            throw new ApplicationException("Invalid token data in session.", ex);
        }
    }
}
