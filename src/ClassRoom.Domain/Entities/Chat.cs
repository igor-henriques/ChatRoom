namespace ChatRoom.Domain.Entities;

public sealed record Chat
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public DateTime CreatedDate { get; init; }
    public string? CreatedByUserId { get; init; }
    public ExtendedIdentityUser? CreatedByUser { get; init; }
    public ICollection<ChatMessage> Messages { get; init; } = [];
}