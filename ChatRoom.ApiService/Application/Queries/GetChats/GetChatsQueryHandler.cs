namespace ChatRoom.ApiService.Application.Queries.GetChats;

public sealed class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, IEnumerable<ChatDto>>
{
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<GetChatsQueryHandler> _logger;

    public GetChatsQueryHandler(IChatRepository chatRepository, ILogger<GetChatsQueryHandler> logger)
    {
        _chatRepository = chatRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ChatDto>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Chat?> chats = await _chatRepository.GetChatsAsync(cancellationToken);
        _logger.LogInformation("{ChatCount} chats were retrieved", chats.Count());
        return chats.Select(chat => (ChatDto)chat!);
    }
}
