using ChatRoom.Domain.Models.Dtos;

namespace ChatRoom.Messages.PersistenceConsumer.Services;

internal interface IChatRoomApiClient
{
    Task<ChatMessageDto> CreateChatMessageAsync(ChatMessageDto chatMessageDto);
}
