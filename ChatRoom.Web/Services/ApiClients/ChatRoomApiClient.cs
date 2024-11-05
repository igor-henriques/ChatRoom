using ChatRoom.Domain.Models.Dtos;

namespace ChatRoom.Web.Services.ApiClients;

public sealed class ChatRoomApiClient(HttpClient httpClient) : IChatRoomApiClient
{
    public async Task<IEnumerable<ChatDto>> GetChatsAsync(CancellationToken cancellationToken = default)
    {
        var chats = await httpClient.GetFromJsonAsync<IEnumerable<ChatDto>>("/chats", cancellationToken) ?? [];
        return chats;
    }

    public async Task<IEnumerable<ChatMessageDto>> GetChatHistoryAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        var chatHistory = await httpClient.GetFromJsonAsync<IEnumerable<ChatMessageDto>>($"/chat-history?chatId={chatId}", cancellationToken) ?? [];
        return chatHistory;
    }
    public async Task<ChatDto> CreateChatAsync(string chatName, string authenticatedUserId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/chat", new { Name = chatName, CreatedDate = DateTime.UtcNow, CreatedByUserId = authenticatedUserId },
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ChatDto>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to parse chat creation response.");
    }

    public async Task DeleteChatAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/chat/{chatId}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}