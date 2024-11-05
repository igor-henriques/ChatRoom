using ChatRoom.Domain.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using ClientWebSocket = ChatRoom.Infrastructure.Services.ClientWebSocket;

namespace ChatRoom.Web.Components.Pages;

[Authorize]
public partial class Chat : IDisposable
{
    [Parameter]
    public Guid ChatId { get; init; }

    [Inject]
    private IPublisher? Publisher { get; init; }

    [Inject]
    private ILogger<Chat>? Logger { get; init; }

    [Inject]
    private IAuthenticatedUserProvider AuthenticatedUserProvider { get; init; } = default!;

    [Inject]
    private IChatRoomApiClient? ApiClient { get; init; }

    [Inject]
    private IConfiguration? Configuration { get; init; }

    [Inject]
    private ISessionTokenService? SessionTokenService { get; init; }

    [Inject]
    private NavigationManager? NavigationManager { get; init; }

    private readonly List<string> ConnectedUsers = [];
    private readonly List<ChatMessageViewModel> Messages = [];
    private string CurrentMessage = string.Empty;
    private ClientWebSocket? _clientWebSocket;
    private AuthenticatedUser? _authenticatedUser;

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentUserName();
        await LoadInitialData();

        var apiServiceHost = new Uri(Configuration!["services:apiservice:https:0"]!);
        _clientWebSocket = ClientWebSocket.Create(new Uri($"wss://{apiServiceHost.Authority}/ws"));
        _clientWebSocket.OnMessageReceived += ConcatenateMessage;

        var tokenResponse = SessionTokenService!.GetTokenResponse();
        if (!tokenResponse.IsTokenValid)
        {
            NavigationManager?.NavigateTo("identity/account/login");
        }

        await _clientWebSocket.ConnectAsync(tokenResponse.AccessToken!);
    }

    private async Task LoadCurrentUserName()
    {
        _authenticatedUser = await AuthenticatedUserProvider.GetAuthenticatedUserAsync();
        string? authenticatedUserName = _authenticatedUser.Name?.Split('@')[0];

        if (!string.IsNullOrWhiteSpace(authenticatedUserName))
        {
            ConnectedUsers.Add(authenticatedUserName);
        }
    }

    private async Task LoadInitialData()
    {
        try
        {
            var chatHistory = await ApiClient!.GetChatHistoryAsync(ChatId);

            foreach (var chatMessage in chatHistory)
            {
                AddChatMessage(chatMessage);
            }
        }
        catch (UnauthorizedAccessException)
        {
            NavigationManager?.NavigateTo("identity/account/login");
        }
    }

    private void AddChatMessage(ChatMessageDto chatMessage)
    {
        var chatModel = new ChatMessageViewModel
        {
            Text = chatMessage.Content,
            User = chatMessage.CreatedByUser!.FilteredUserName
        };

        if (chatMessage.CreatedByUserId == _authenticatedUser!.UserId.ToString())
        {
            chatModel = chatModel with { User = "You" };
        }

        Messages.Add(chatModel);
    }

    private async Task ConcatenateMessage(string message)
    {
        var messageDto = JsonConvert.DeserializeObject<ChatMessageDto>(message);
        if (messageDto?.CreatedByUser is null)
        {
            Logger?.LogError("Invalid user interacted with the chat");
            return;
        }

        AddChatMessage(messageDto);

        await InvokeAsync(StateHasChanged);
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrEmpty(CurrentMessage) || string.IsNullOrWhiteSpace(_authenticatedUser?.Name))
        {
            return;
        }

        try
        {
            ChatMessageDto message = new()
            {
                Content = CurrentMessage,
                ChatId = ChatId,
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = _authenticatedUser.UserId.ToString(),
                CreatedByUser = new UserDto() { UserName = _authenticatedUser.Name, Id = _authenticatedUser.UserId.ToString() }
            };

            Publisher?.PublishMessage(JsonConvert.SerializeObject(message));
            Messages.Add(new ChatMessageViewModel { User = "You", Text = CurrentMessage });
            Logger?.LogInformation("Message {CurrentMessage} successfully broadcasted", CurrentMessage);
            CurrentMessage = string.Empty;
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "An error occurred while user {UserId} tried to send a message", _authenticatedUser.UserId);
        }
    }

    public void Dispose()
    {
        if (_clientWebSocket is not null)
        {
            _clientWebSocket.Dispose();
            _clientWebSocket = null;
            Logger?.LogInformation("WebSocket connection closed and disposed");
        }

        GC.SuppressFinalize(this);
    }
}