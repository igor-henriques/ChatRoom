using ChatRoom.Infrastructure.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ChatRoom.Web.Services;

public sealed class ChatMessagePublisher : TopicPublisher, IPublisher
{
    public ChatMessagePublisher(IConnectionFactory connectionFactory, IOptions<AppDefinitions> options, ILogger<ChatMessagePublisher> logger)
        : base(connectionFactory, logger, options.Value.ExchangeName!)
    {
    }
}
