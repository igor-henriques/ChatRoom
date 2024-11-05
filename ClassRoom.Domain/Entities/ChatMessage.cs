namespace ChatRoom.Domain.Entities;

public sealed record ChatMessage
{
    public Guid Id { get; init; }
    public Guid ChatId { get; init; }
    public string? Content { get; init; }
    public DateTime CreatedDate { get; init; }
    public string? CreatedByUserId { get; init; }
    public ExtendedIdentityUser? CreatedByUser { get; init; }
}