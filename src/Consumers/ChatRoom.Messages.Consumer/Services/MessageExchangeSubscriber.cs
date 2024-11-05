using ChatRoom.Domain.Models.Dtos;
using ChatRoom.Domain.Services;
using ChatRoom.Domain.Shared;
using ChatRoom.Infrastructure.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ChatRoom.Messages.PersistenceConsumer.Services;

internal sealed class MessageExchangeSubscriber : TopicSubscriber, ISubscriber
{
    private readonly IChatRoomApiClient _chatRoomApiClient;
    private readonly ILogger<MessageExchangeSubscriber> _logger;

    public MessageExchangeSubscriber(
        IConnectionFactory connectionFactory,
        IChatRoomApiClient chatRoomApiClient,
        ILogger<TopicSubscriber> logger,
        ILogger<MessageExchangeSubscriber> innerLogger) : base(connectionFactory, logger)
    {
        _chatRoomApiClient = chatRoomApiClient;
        _logger = innerLogger;
    }

    protected override string ExchangeName => "chat.messages.exchange";

    protected override async Task HandleMessageAsync(string message)
    {
        var chatMessageDto = JsonConvert.DeserializeObject<ChatMessageDto>(message);

        if (StockHelper.IsStockCommand(chatMessageDto!.Content!))
        {
            _logger.LogInformation("Stock commands should not persist.");
            return;
        }

        await _chatRoomApiClient.CreateChatMessageAsync(chatMessageDto!);
        _logger.LogInformation("Message successfully persisted");
    }
}
