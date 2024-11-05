using ChatRoom.Domain.Models;

namespace ChatRoom.Web.Services;

public interface ISessionTokenService
{
    TokenResponse GetTokenResponse();
}
