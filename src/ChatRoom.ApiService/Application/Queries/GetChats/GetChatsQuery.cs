namespace ChatRoom.ApiService.Application.Queries.GetChats;

public sealed record GetChatsQuery : IRequest<IEnumerable<ChatDto>>
{
}
