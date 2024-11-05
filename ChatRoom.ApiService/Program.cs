using ChatRoom.ApiService.StartupConfiguration;
using Serilog;

try
{
    await WebApplication.CreateBuilder(args)
        .ConfigureServices()
        .Build()
        .ConfigurePipelines()
        .RunAsync();
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