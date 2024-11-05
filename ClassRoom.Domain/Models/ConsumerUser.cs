namespace ChatRoom.Domain.Models;

public sealed record ConsumerUser
{
    public string? Email { get; init; }
    public string? Password { get; init; }
}
