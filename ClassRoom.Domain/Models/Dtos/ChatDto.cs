using ChatRoom.Domain.Entities;

namespace ChatRoom.Domain.Models.Dtos;

public sealed class ChatDto
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
    public DateTime CreatedDate { get; init; }
    public string? CreatedByUserId { get; init; }
    public UserDto? CreatedByUser { get; init; }
    public IEnumerable<ChatMessageDto>? Messages { get; init; }

    public static explicit operator ChatDto(Chat chat)
    {
        return new ChatDto()
        {
            CreatedByUser = (UserDto?)chat.CreatedByUser,
            CreatedByUserId = chat.CreatedByUserId,
            CreatedDate = chat.CreatedDate,
            Messages = chat.Messages.Select(message => (ChatMessageDto)message!),
            Name = chat.Name,
            Id = chat.Id
        };
    }
}