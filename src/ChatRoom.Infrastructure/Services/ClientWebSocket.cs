using Serilog;
using System.Net.WebSockets;
using System.Text;

namespace ChatRoom.Infrastructure.Services;

public sealed class ClientWebSocket : IDisposable
{
    private System.Net.WebSockets.ClientWebSocket? _webSocket;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Uri _serverUri;

    public event Func<string, Task>? OnMessageReceived;

    private ClientWebSocket(Uri serverUri)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _serverUri = serverUri;
    }

    public static ClientWebSocket Create(Uri serverUri)
    {
        return new ClientWebSocket(serverUri);
    }

    public async Task ConnectAsync(string accessToken)
    {
        _webSocket = new System.Net.WebSockets.ClientWebSocket();
        _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {accessToken}");

        try
        {
            await _webSocket.ConnectAsync(_serverUri, _cancellationTokenSource.Token);
            _ = ReceiveMessagesAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Failed to connect to WebSocket");
        }
    }

    public async Task SendAsync(string message)
    {
        if (_webSocket?.State == WebSocketState.Open)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        }
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024 * 4];

        while (_webSocket?.State == WebSocketState.Open)
        {
            try
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (OnMessageReceived != null)
                    {
                        await OnMessageReceived.Invoke(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error receiving message");
                break;
            }
        }
    }

    public void Dispose()
    {
        _webSocket?.Dispose();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}