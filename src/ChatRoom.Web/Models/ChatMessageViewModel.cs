namespace ChatRoom.Web.Models;

public sealed record ChatMessageViewModel
{
    public string? User { get; init; }
    public string? Text { get; init; }
}
