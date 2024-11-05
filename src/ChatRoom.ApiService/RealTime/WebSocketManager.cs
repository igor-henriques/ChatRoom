using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ChatRoom.ApiService.RealTime;

public static class WebSocketManager
{
    private static readonly ConcurrentDictionary<string, WebSocket> ActiveWebSockets = new();

    public static void AddWebSocket(string userId, WebSocket webSocket)
    {
        ActiveWebSockets[userId] = webSocket;
    }

    public static void RemoveWebSocket(string userId)
    {
        ActiveWebSockets.TryRemove(userId, out _);
    }

    public static IEnumerable<WebSocket> GetAllWebSocketsExcept(string senderUserId)
    {
        return ActiveWebSockets
            .Where(kvp => kvp.Key != senderUserId && kvp.Value.State == WebSocketState.Open)
            .Select(kvp => kvp.Value);
    }
}
