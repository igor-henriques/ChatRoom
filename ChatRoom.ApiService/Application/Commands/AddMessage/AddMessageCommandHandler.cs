using Newtonsoft.Json;

namespace ChatRoom.ApiService.Application.Commands.AddMessage;

public sealed class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, ChatMessageDto>
{
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<AddMessageCommandHandler> _logger;
    private readonly Domain.Services.IPublisher _publisher;

    public AddMessageCommandHandler(IChatRepository chatRepository, ILogger<AddMessageCommandHandler> logger, Domain.Services.IPublisher publisher)
    {
        _chatRepository = chatRepository;
        _logger = logger;
        _publisher = publisher;
    }

    public async Task<ChatMessageDto> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ChatId = request.ChatId,
            Content = request.Content,
            CreatedDate = request.CreatedDate,
            CreatedByUserId = request.CreatedByUserId
        };

        _logger.LogInformation("Adding message to chat {ChatId}", request.ChatId);

        _ = await _chatRepository.GetChatAsync(chatMessage.ChatId, cancellationToken)
            ?? throw new InvalidChatException();

        var savedMessage = await _chatRepository.AddChatMessageAsync(chatMessage, cancellationToken);
        var addedChatMessage = await _chatRepository.GetChatMessageAsync(savedMessage.Id, cancellationToken);
        string serializedChatMessage = JsonConvert.SerializeObject(addedChatMessage);
        _publisher.PublishMessage(serializedChatMessage);

        return (ChatMessageDto)addedChatMessage!;
    }
}
