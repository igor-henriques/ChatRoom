using ChatRoom.Domain.Models.Dtos;
using ChatRoom.Domain.Services;
using ChatRoom.Domain.Shared;
using ChatRoom.Infrastructure.Services;
using ChatRoom.Messages.BotConsumer.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Globalization;

namespace ChatRoom.Messages.BotConsumer.Services;

internal sealed class BotSubscriber : TopicSubscriber, ISubscriber
{
    private readonly IStooqApiClient _stooqApiClient;
    private readonly ILogger<BotSubscriber> _logger;
    private readonly IPublisher _publisher;

    private readonly AppDefinitions _options;

    public BotSubscriber(
        IConnectionFactory connectionFactory,
        IStooqApiClient chatRoomApiClient,
        ILogger<TopicSubscriber> logger,
        ILogger<BotSubscriber> innerLogger,
        IOptions<AppDefinitions> options,
        IPublisher publisher) : base(connectionFactory, logger)
    {
        _stooqApiClient = chatRoomApiClient;
        _logger = innerLogger;
        _options = options.Value;
        _publisher = publisher;
    }

    protected override string ExchangeName => "chat.messages.exchange";

    protected override async Task HandleMessageAsync(string message)
    {
        var chatMessageDto = JsonConvert.DeserializeObject<ChatMessageDto>(message);
        if (!StockHelper.IsStockCommand(chatMessageDto!.Content!))
        {
            _logger.LogInformation("Ignoring message since it is not a stock command.");
            return;
        }

        string stock = chatMessageDto.Content!.GetStockName();
        decimal stockValue = await _stooqApiClient.GetStockQuoteAsync(stock);

        _logger.LogInformation("Stock for {StockName} successfully fetched with value of {StockValue}",
            stock,
            stockValue);

        string response = string.Format(
          _options.StockResponseMessage!,
          stock,
          stockValue.ToString("c", CultureInfo.GetCultureInfo("en-US")));

        var chatMessage = new ChatMessageDto
        {
            Id = Guid.NewGuid(),
            Content = response,
            CreatedDate = DateTime.UtcNow,
            ChatId = chatMessageDto.ChatId,
            CreatedByUser = new UserDto()
            {
                UserName = "StockBot"
            },
        };

        _publisher.PublishMessage(JsonConvert.SerializeObject(chatMessage));
    }
}
