namespace ChatRoom.ApiService.Application.Commands.CreateChat;

public sealed record CreateChatCommand : IRequest<ChatDto>
{
    public string? Name { get; init; }
    public DateTime CreatedDate { get; init; }
    public string? CreatedByUserId { get; init; }
}
