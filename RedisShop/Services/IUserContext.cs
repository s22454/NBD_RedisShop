
namespace RedisShop.Services;

public interface IUserContext
{
    string UserId { get; }
    bool IsAuthenticated { get; }
}
