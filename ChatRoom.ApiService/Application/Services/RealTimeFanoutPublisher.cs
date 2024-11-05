using ChatRoom.ApiService.Models;
using ChatRoom.Infrastructure.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ChatRoom.ApiService.Application.Services;

public sealed class RealTimeFanoutPublisher : QueuePublisher, Domain.Services.IPublisher
{
    public RealTimeFanoutPublisher(IConnectionFactory connectionFactory, IOptions<AppDefinitions> options, ILogger<RealTimeFanoutPublisher> logger)
        : base(connectionFactory, logger, options.Value.FanoutQueueName!)
    {
    }

    public void PublishChatMessage(string messageContent)
    {
        PublishMessage(messageContent);
    }
}
