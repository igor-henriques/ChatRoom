using ChatRoom.Domain.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ChatRoom.Web.Components.Pages;

[Authorize]
public partial class ChatList
{
    [Inject]
    private IChatRoomApiClient? ApiClient { get; init; }

    [Inject]
    private ILogger<ChatList>? Logger { get; init; }

    [Inject]
    private AuthenticationStateProvider? AuthenticationStateProvider { get; init; }

    [Inject]
    private NavigationManager? NavigationManager { get; init; }

    [Inject]
    private ISessionTokenService? SessionTokenService { get; init; }

    protected internal List<ChatDto> Chats = [];
    private string NewChatName = string.Empty;
    private string? CurrentUserId;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated ?? false)
            {
                CurrentUserId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }

            var tokenResponse = SessionTokenService!.GetTokenResponse();

            Chats = (await ApiClient!.GetChatsAsync()).ToList();
        }
        catch (JsonException)
        {
            NavigationManager?.NavigateTo("identity/account/login", true);
        }
        catch (UnauthorizedAccessException)
        {
            NavigationManager?.NavigateTo("identity/account/login", true);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error loading chats");
        }
    }

    private async Task CreateChatAsync()
    {
        if (string.IsNullOrEmpty(NewChatName) || string.IsNullOrWhiteSpace(CurrentUserId))
        {
            Logger?.LogWarning("Invalid chat name or current user id not found");
            return;
        }

        try
        {
            ChatDto newChat = await ApiClient!.CreateChatAsync(NewChatName, CurrentUserId);
            Chats.Add(newChat);
            NewChatName = string.Empty;
            NavigationManager?.NavigateTo($"/chat/{newChat.Id}");
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error creating chat");
        }
    }

    private async Task DeleteChatAsync(Guid chatId)
    {
        try
        {
            await ApiClient!.DeleteChatAsync(chatId);
            Chats.RemoveAll(chat => chat.Id == chatId);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error deleting chat");
        }
    }
}
