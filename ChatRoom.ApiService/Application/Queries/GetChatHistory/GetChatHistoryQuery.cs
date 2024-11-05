namespace ChatRoom.ApiService.Application.Queries.GetChatHistory;

public sealed record GetChatHistoryQuery : IRequest<IEnumerable<ChatMessageDto>>
{
    public Guid ChatId { get; init; }
}
