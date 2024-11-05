using ChatRoom.Domain.Models;
using ChatRoom.Domain.Services;
using ChatRoom.Infrastructure.HttpHandlers;
using ChatRoom.Infrastructure.Services;
using ChatRoom.Messages.Common.HttpHandlers;
using ChatRoom.Messages.PersistenceConsumer;
using ChatRoom.Messages.PersistenceConsumer.Services;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.ConfigureLogging();
    builder.AddServiceDefaults();
    builder.AddRabbitMQClient("chatbroker");

    builder.Services.AddScoped<RetryHandler>();
    builder.Services.AddScoped<AuthHandler>();
    builder.Services.Configure<ConsumerUser>(builder.Configuration.GetSection(nameof(ConsumerUser)));
    builder.Services.AddSingleton<ISubscriber, MessageExchangeSubscriber>();

    builder.Services.AddHttpClient<IChatRoomApiClient, ChatRoomApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    })
    .AddHttpMessageHandler<AuthHandler>();

    builder.Services.AddHttpClient<ITokenProviderApiClient, TokenProviderApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
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