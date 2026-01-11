
using Redis.OM;
using Redis.OM.Searching;
using RedisShop.Models;

namespace RedisShop.Services;

public class UserService : IUserService
{
    private static readonly string _className = "UserService";
    private readonly RedisCollection<User> _users;

    public UserService(RedisConnectionProvider provider)
    {
        _users = (RedisCollection<User>)provider.RedisCollection<User>();
    }

    public async Task RegisterAsync(string login, string password)
    {
        //todo add log and excaptions

        // Get user with matching login
        var existing = await _users
            .Where(u => u.Login == login)
            .FirstOrDefaultAsync();

        // Exit if user with that login exists
        if (existing is null)
        {
            Utils.Logger.Log(_className, LogType.ERROR, "User with that login already exist!");
        }

        // Add user to db
        await _users.InsertAsync(new User()
        {
            Login = login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        });
    }

    public async Task<User?> LoginAsync(string login, string password)
    {
        //todo add log and excaptions

        // Get user with matching login
        var user = await _users
            .Where(u => u.Login == login)
            .FirstOrDefaultAsync();

        // Return null if user doesn't exist
        if (user is null) return null;

        // Verify password
        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        return isValid ? user : null;
    }
}
