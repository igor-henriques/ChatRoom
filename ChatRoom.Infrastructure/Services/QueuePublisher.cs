using ChatRoom.Domain.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;

namespace ChatRoom.Infrastructure.Services;

public abstract class QueuePublisher : IPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<QueuePublisher> _logger;
    private readonly string _queueName;

    public QueuePublisher(IConnectionFactory connectionFactory, ILogger<QueuePublisher> logger, string queueName)
    {
        _logger = logger;
        _queueName = queueName;
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublishMessage(string message)
    {
        byte[] body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
                              routingKey: _queueName,
                              basicProperties: null,
                              body: body);

        _logger.LogInformation("A message was successfully posted into RabbitMQ queue: {QueueName}", _queueName);
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
