using ChatRoom.Domain.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;

namespace ChatRoom.Infrastructure.Services;

public abstract class TopicPublisher : IPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<TopicPublisher> _logger;
    private readonly string _exchangeName;

    public TopicPublisher(IConnectionFactory connectionFactory, ILogger<TopicPublisher> logger, string exchangeName)
    {
        _logger = logger;
        _exchangeName = exchangeName;
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false);
    }

    public void PublishMessage(string message)
    {
        byte[] body = Encoding.UTF8.GetBytes(message);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: _exchangeName,
                              routingKey: "",
                              basicProperties: properties,
                              body: body);

        _logger.LogInformation("A message was successfully posted into RabbitMQ exchange: {ExchangeName}", _exchangeName);
    }

    public void Dispose()
    {
        if (_channel != null)
        {
            _channel.Close();
            _channel.Dispose();
        }

        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
