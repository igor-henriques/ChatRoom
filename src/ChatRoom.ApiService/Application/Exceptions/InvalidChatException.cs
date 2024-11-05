using ChatRoom.Domain.Exceptions;

namespace ChatRoom.ApiService.Application.Exceptions;

public sealed class InvalidChatException : DomainException
{
    public InvalidChatException() : base("Invalid chat.")
    {

    }
}
