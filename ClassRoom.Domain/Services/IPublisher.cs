namespace ChatRoom.Domain.Services;

public interface IPublisher
{
    void PublishMessage(string message);
}
