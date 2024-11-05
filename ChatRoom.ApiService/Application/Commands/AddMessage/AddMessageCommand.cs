namespace ChatRoom.ApiService.Application.Commands.AddMessage;

public sealed record AddMessageCommand : IRequest<ChatMessageDto>
{
    public Guid ChatId { get; init; }
    public string? Content { get; init; }
    public DateTime CreatedDate { get; init; }
    public string? CreatedByUserId { get; init; }
}
