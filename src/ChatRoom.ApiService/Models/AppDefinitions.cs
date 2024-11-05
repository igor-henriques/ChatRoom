namespace ChatRoom.ApiService.Models;

public sealed record AppDefinitions
{
    public int MessagesFetchHistorySize { get; init; }
    public string? FanoutQueueName { get; init; }
}
