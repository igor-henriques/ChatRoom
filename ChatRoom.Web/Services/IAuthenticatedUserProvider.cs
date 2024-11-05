namespace ChatRoom.Web.Services;

public interface IAuthenticatedUserProvider
{
    Task<AuthenticatedUser> GetAuthenticatedUserAsync();
}
