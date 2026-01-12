
namespace RedisShop.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }

    public string UserId
    {
        get
        {
            // Get user
            var user = _httpContextAccessor.HttpContext?.User;

            // Get id from claims
            var id = user?.FindFirst("RedisUserId")?.Value;

            // Check dose user is logged in
            if (string.IsNullOrEmpty(id))
                throw new UnauthorizedAccessException("There is no logged user!");

            return id;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor
        .HttpContext
        ?.User
        ?.Identity
        ?.IsAuthenticated
        ?? false;
}
