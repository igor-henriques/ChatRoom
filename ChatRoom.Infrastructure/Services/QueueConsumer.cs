using ChatRoom.Domain.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatRoom.Infrastructure.Services;

public abstract class QueueConsumer : ISubscriber, IDisposable
{
    protected abstract string QueueName { get; }

    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<QueueConsumer> _logger;
    private readonly EventingBasicConsumer _consumer;

    public QueueConsumer(IConnectionFactory connectionFactory, ILogger<QueueConsumer> logger)
    {
        _logger = logger;

        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Message received from queue '{QueueName}': {Message}", QueueName, message);

            try
            {
                await HandleMessageAsync(message);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while consuming a message from queue '{QueueName}'", QueueName);
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };
    }

    protected abstract Task HandleMessageAsync(string message);

    public void StartConsuming()
    {
        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: _consumer);
        Console.ReadKey();
    }

    public void Dispose()
    {
        _channel.Close();
        _channel.Dispose();
        _connection.Close();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
