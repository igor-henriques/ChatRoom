using ChatRoom.Domain.Services;
using System.Diagnostics;

namespace ChatRoom.Messages.RealTimeFanout;

public sealed class Worker(
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<Worker> logger,
    ISubscriber queueSubscriber) : BackgroundService
{
    public const string ActivitySourceName = "ChatRoom.Messages.Fanout";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Consuming message", ActivityKind.Client);

        try
        {
            queueSubscriber.StartConsuming();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while consuming messages.");
            throw;
        }

        hostApplicationLifetime.StopApplication();
        return Task.CompletedTask;
    }
}