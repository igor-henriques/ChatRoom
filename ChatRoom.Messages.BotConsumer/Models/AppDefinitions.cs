namespace ChatRoom.Messages.BotConsumer.Models;

internal sealed record AppDefinitions
{
    public string? StockResponseMessage { get; init; }
    public string? QueueName { get; init; }
    public string? RealTimeFanoutQueueName { get; init; }
}
