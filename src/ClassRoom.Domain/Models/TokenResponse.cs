namespace ChatRoom.Domain.Models;

public sealed record TokenResponse
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public string? TokenType { get; init; }
    public int ExpiresIn { get; init; }
    public DateTime IssuedAt { get; init; }

    public bool IsTokenValid => DateTime.UtcNow < IssuedAt.AddSeconds(ExpiresIn);
}
