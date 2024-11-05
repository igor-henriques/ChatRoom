using ChatRoom.Domain.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatRoom.Infrastructure.Services;

public abstract class TopicSubscriber : ISubscriber, IDisposable
{
    protected abstract string ExchangeName { get; }

    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<TopicSubscriber> _logger;

    private readonly string _queueName;
    private readonly EventingBasicConsumer _consumer;

    public TopicSubscriber(IConnectionFactory connectionFactory, ILogger<TopicSubscriber> logger)
    {
        connectionFactory.RequestedHeartbeat = TimeSpan.FromSeconds(60);

        _logger = logger;

        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false);

        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName, exchange: ExchangeName, routingKey: "");

        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Message received from exchange '{ExchangeName}': {Message}", ExchangeName, message);

            try
            {
                await HandleMessageAsync(message);                
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while consuming a message from exchange '{ExchangeName}'", ExchangeName);                
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };
    }

    protected abstract Task HandleMessageAsync(string message);

    public void StartConsuming()
    {
        // Muda o autoAck para false para confirmação manual
        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: _consumer);
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
