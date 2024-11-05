using ChatRoom.Domain.Models.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace ChatRoom.Messages.PersistenceConsumer.Services;

internal sealed class ChatRoomApiClient(HttpClient httpClient) : IChatRoomApiClient
{
    public async Task<ChatMessageDto> CreateChatMessageAsync(ChatMessageDto chatMessageDto)
    {
        var response = await httpClient.PostAsJsonAsync("/message", new
        {
            chatMessageDto.ChatId,
            chatMessageDto.Content,
            chatMessageDto.CreatedDate,
            chatMessageDto.CreatedByUserId
        });

        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<ChatMessageDto>(await response.Content.ReadAsStringAsync())!;
    }
}
