using ChatRoom.Domain.Entities;

namespace ChatRoom.Domain.Models.Dtos;

public sealed record UserDto
{
    public string? Id { get; init; }
    public string? UserName { get; init; }
    public string? FilteredUserName => UserName!.Contains('@') ? UserName!.Split('@')[0] : UserName;

    public static explicit operator UserDto?(ExtendedIdentityUser? user)
    {
        if (user is null)
        {
            return null;
        }

        return new()
        {
            Id = user.Id,
            UserName = user.NormalizedUserName
        };
    }
}
