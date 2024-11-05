using Serilog;

namespace ChatRoom.MigrationService;

public static class LoggingConfigurator
{
    public static HostApplicationBuilder ConfigureLogging(this HostApplicationBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration);

        var serilogLogger = loggerConfiguration.CreateLogger();       

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(serilogLogger, true).AddConsole();
        Log.Logger = serilogLogger;

        return builder;
    }
}
