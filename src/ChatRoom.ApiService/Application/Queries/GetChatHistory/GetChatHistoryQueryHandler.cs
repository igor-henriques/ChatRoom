using ChatRoom.ApiService.Models;
using Microsoft.Extensions.Options;

namespace ChatRoom.ApiService.Application.Queries.GetChatHistory;

public sealed class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, IEnumerable<ChatMessageDto>>
{
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<GetChatHistoryQueryHandler> _logger;
    private readonly AppDefinitions _options;

    public GetChatHistoryQueryHandler(IChatRepository chatRepository, ILogger<GetChatHistoryQueryHandler> logger, IOptions<AppDefinitions> options)
    {
        _chatRepository = chatRepository;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<IEnumerable<ChatMessageDto>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var chatMessages = await _chatRepository.GetChatMessagesAsync(request.ChatId, _options.MessagesFetchHistorySize, cancellationToken);
        _logger.LogInformation("{ChatCount} chat messages were retrieved", chatMessages.Count());
        return chatMessages.Select(message => (ChatMessageDto)message!);
    }
}
