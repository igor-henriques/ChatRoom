using ChatRoom.Domain.Models;
using ChatRoom.Domain.Services;
using ChatRoom.Infrastructure.HttpHandlers;
using ChatRoom.Infrastructure.Services;
using ChatRoom.Messages.Common.HttpHandlers;
using ChatRoom.Messages.Fanout;
using ChatRoom.Messages.RealTimeFanout;
using ChatRoom.Messages.RealTimeFanout.Services;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.ConfigureLogging();
    builder.AddServiceDefaults();
    builder.AddRabbitMQClient("chatbroker");

    builder.Services.Configure<ConsumerUser>(builder.Configuration.GetSection(nameof(ConsumerUser)));
    builder.Services.AddScoped<AuthHandler>();
    builder.Services.AddScoped<RetryHandler>();
    builder.Services.Configure<ConsumerUser>(builder.Configuration.GetSection(nameof(ConsumerUser)));
    builder.Services.AddSingleton<ISubscriber, RealTimeFanoutConsumer>();

    builder.Services.AddHttpClient<ITokenProviderApiClient, TokenProviderApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    }).AddHttpMessageHandler<RetryHandler>();

    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();

    await Task.Delay(TimeSpan.FromSeconds(5));

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex.ToString());
    throw;
}
finally
{
    Log.CloseAndFlush();
}