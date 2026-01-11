
using RedisShop.Models;

namespace RedisShop.Services;

public interface IUserService
{
    Task RegisterAsync(string login, string password);
    Task<User?> LoginAsync(string login, string password);
}
