namespace ChatRoom.Web.Models;

public sealed record AuthenticatedUser
{
    public Guid UserId { get; init; }
    public string? Name { get; init; }
}
