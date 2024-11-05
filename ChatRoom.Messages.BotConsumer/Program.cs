using ChatRoom.Domain.Services;
using ChatRoom.Infrastructure.HttpHandlers;
using ChatRoom.Messages.BotConsumer;
using ChatRoom.Messages.BotConsumer.Models;
using ChatRoom.Messages.BotConsumer.Services;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.ConfigureLogging();
    builder.AddServiceDefaults();
    builder.AddRabbitMQClient("chatbroker");

    builder.Services.AddSingleton<IPublisher, ChatMessagePublisher>();
    builder.Services.AddSingleton<ISubscriber, BotSubscriber>();

    builder.Services.AddScoped<RetryHandler>();
    builder.Services.Configure<AppDefinitions>(builder.Configuration.GetSection(nameof(AppDefinitions)));

    builder.Services.AddHttpClient<IStooqApiClient, StooqApiClient>((client) =>
    {
        client.BaseAddress = new(builder.Configuration["AppDefinitions:StooqBaseUrl"]!);
    }).AddHttpMessageHandler<RetryHandler>();

    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();

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