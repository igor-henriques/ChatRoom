namespace ChatRoom.ApiService.Application.Commands.DeleteChat;

public sealed class DeleteChatCommandHandler : IRequestHandler<DeleteChatCommand, Unit>
{
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<DeleteChatCommandHandler> _logger;

    public DeleteChatCommandHandler(IChatRepository chatRepository, ILogger<DeleteChatCommandHandler> logger)
    {
        _chatRepository = chatRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        await _chatRepository.DeleteChatAsync(request.ChatId, cancellationToken);
        _logger.LogInformation("Chat {ChatId} has been deleted", request.ChatId);
        return Unit.Value;
    }
}
