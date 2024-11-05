using ChatRoom.ApiService.Endpoints;
using ChatRoom.ApiService.Middlewares;
using ChatRoom.ApiService.RealTime;
using System.Diagnostics;

namespace ChatRoom.ApiService.StartupConfiguration;

public static class PipelinesConfigurator
{
    public static WebApplication ConfigurePipelines(this WebApplication app)
    {
        app.UseMiddleware<CorrelationMiddleware>();

        if (Debugger.IsAttached)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapIdentityApi<ExtendedIdentityUser>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.ConfigureRealTimeEndpoint();
        app.UseExceptionHandler();
        app.MapDefaultEndpoints();
        app.ConfigureChatEndpoints();

        return app;
    }
}
