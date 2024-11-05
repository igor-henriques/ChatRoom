using ChatRoom.Domain.Models;

namespace ChatRoom.Domain.Services;

public interface ITokenProviderApiClient
{
    Task<TokenResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<TokenResponse> RefreshTokenAsync(TokenResponse token, CancellationToken cancellationToken = default);
}
