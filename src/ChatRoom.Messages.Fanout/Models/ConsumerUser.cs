namespace ChatRoom.Messages.RealTimeFanout.Models;

internal sealed record ConsumerUser
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
