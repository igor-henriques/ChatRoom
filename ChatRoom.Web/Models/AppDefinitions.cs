namespace ChatRoom.Web.Models;

public sealed record AppDefinitions
{
    public string? ExchangeName { get; init; }
    public string? BotResponseQueueName { get; init; }
    public string? RealTimeFanoutQueueName { get; init; }
}
