namespace ChatRoom.ApiService.Application.Commands.CreateChat;

public sealed class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, ChatDto>
{
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<CreateChatCommandHandler> _logger;

    public CreateChatCommandHandler(IChatRepository chatRepository, ILogger<CreateChatCommandHandler> logger)
    {
        _chatRepository = chatRepository;
        _logger = logger;
    }

    public async Task<ChatDto> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            CreatedByUserId = request.CreatedByUserId,
            Name = request.Name,
            CreatedDate = request.CreatedDate
        };

        _logger.LogInformation("Creating chat {ChatId}", chat.Id);

        var addedChat = await _chatRepository.AddChatAsync(chat, cancellationToken);
        return (ChatDto)addedChat;
    }
}
