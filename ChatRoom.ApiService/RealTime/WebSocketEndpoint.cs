using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

namespace ChatRoom.ApiService.RealTime;

public static class WebSocketEndpoint
{
    public static void ConfigureRealTimeEndpoint(this WebApplication app)
    {
        app.UseWebSockets();

        app.Map("/ws", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest && context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (userId != null)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    WebSocketManager.AddWebSocket(userId, webSocket);

                    try
                    {
                        await HandleWebSocketConnection(userId, webSocket);
                    }
                    finally
                    {
                        WebSocketManager.RemoveWebSocket(userId);
                    }
                }
                else
                {
                    context.Response.StatusCode = 401;
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }).RequireAuthorization();
    }

    private static async Task HandleWebSocketConnection(string userId, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.CloseStatus.HasValue) break;

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await BroadcastMessage(message, userId);
        } while (!result.CloseStatus.HasValue);

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    private static async Task BroadcastMessage(string message, string senderUserId)
    {
        var messageData = Encoding.UTF8.GetBytes(message);

        foreach (var webSocket in WebSocketManager.GetAllWebSocketsExcept(senderUserId))
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(messageData), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}