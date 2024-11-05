using ChatRoom.Domain.Models.Dtos;

namespace ChatRoom.Web.Services.ApiClients;

public interface IChatRoomApiClient
{
    Task<IEnumerable<ChatDto>> GetChatsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatMessageDto>> GetChatHistoryAsync(Guid chatId, CancellationToken cancellationToken = default);
    Task<ChatDto> CreateChatAsync(string chatName, string authenticatedUserId, CancellationToken cancellationToken = default);
    Task DeleteChatAsync(Guid chatId, CancellationToken cancellationToken = default);
}
