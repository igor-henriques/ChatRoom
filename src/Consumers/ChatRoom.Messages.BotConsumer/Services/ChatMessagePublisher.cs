using ChatRoom.Domain.Services;
using ChatRoom.Infrastructure.Services;
using ChatRoom.Messages.BotConsumer.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ChatRoom.Messages.BotConsumer.Services;

internal sealed class ChatMessagePublisher : QueuePublisher, IPublisher
{
    public ChatMessagePublisher(IConnectionFactory connectionFactory, IOptions<AppDefinitions> options, ILogger<ChatMessagePublisher> logger)
        : base(connectionFactory, logger, options.Value.RealTimeFanoutQueueName!)
    {
    }
}
