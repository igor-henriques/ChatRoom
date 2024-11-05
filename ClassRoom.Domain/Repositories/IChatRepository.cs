using ChatRoom.Domain.Entities;

namespace ChatRoom.Domain.Repositories;

public interface IChatRepository
{
    Task<Chat?> GetChatAsync(Guid chatId, CancellationToken cancellationToken = default);
    Task<Chat> AddChatAsync(Chat chat, CancellationToken cancellationToken = default);
    Task DeleteChatAsync(Guid chatId, CancellationToken cancellationToken = default);
    Task<ChatMessage?> GetChatMessageAsync(Guid chatMessageId, CancellationToken cancellationToken = default);
    Task<ChatMessage> AddChatMessageAsync(ChatMessage chatMessage, CancellationToken cancellationToken = default);
    Task DeleteChatMessageAsync(Guid chatMessageId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatMessage?>> GetChatMessagesAsync(Guid chatId, int historySize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Chat?>> GetChatsAsync(CancellationToken cancellationToken = default);
}
