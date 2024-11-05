using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ChatRoom.Web.Services;

public sealed class AuthenticatedUserProvider : IAuthenticatedUserProvider
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthenticatedUserProvider(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<AuthenticatedUser> GetAuthenticatedUserAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;

        if (!user.Identity?.IsAuthenticated ?? false)
        {
            throw new AuthenticationFailureException("Authentication is required");
        }

        _ = Guid.TryParse(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var userId);

        return new()
        {
            UserId = userId,
            Name = user.Identity!.Name
        };
    }
}
