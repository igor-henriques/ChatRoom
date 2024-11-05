using ChatRoom.Infrastructure.Data;
using ChatRoom.MigrationService;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.ConfigureLogging();
    builder.AddServiceDefaults();
    builder.Services.AddHostedService<Worker>();

    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

    builder.AddNpgsqlDbContext<ApplicationDbContext>("chatroomdb");

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