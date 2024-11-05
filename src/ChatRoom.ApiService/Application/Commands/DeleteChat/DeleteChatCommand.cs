namespace ChatRoom.ApiService.Application.Commands.DeleteChat;

public sealed record DeleteChatCommand : IRequest<Unit>
{
    public DeleteChatCommand(Guid chatId)
    {
        ChatId = chatId;
    }

    public Guid ChatId { get; }
}
