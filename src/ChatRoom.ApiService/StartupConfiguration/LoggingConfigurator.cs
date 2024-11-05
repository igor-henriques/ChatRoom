using Serilog;

namespace ChatRoom.ApiService.StartupConfiguration;

public static class LoggingConfigurator
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
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
