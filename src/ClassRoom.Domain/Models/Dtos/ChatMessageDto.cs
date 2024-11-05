using ChatRoom.Domain.Entities;

namespace ChatRoom.Domain.Models.Dtos;

public sealed class ChatMessageDto
{
    public Guid Id { get; init; }
    public Guid ChatId { get; init; }
    public string? Content { get; init; }
    public DateTime CreatedDate { get; init; }
    public string? CreatedByUserId { get; init; }
    public UserDto? CreatedByUser { get; init; }

    public static explicit operator ChatMessageDto(ChatMessage chatMessage)
    {
        return new ChatMessageDto()
        {
            CreatedByUser = (UserDto?)chatMessage.CreatedByUser,
            Content = chatMessage.Content,
            CreatedByUserId = chatMessage.CreatedByUserId,
            CreatedDate = chatMessage.CreatedDate,
            Id = chatMessage.Id,
            ChatId = chatMessage.ChatId
        };
    }
}
