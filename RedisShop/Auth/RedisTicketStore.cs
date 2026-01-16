using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisShop.Auth;

public class RedisTicketStore : ITicketStore
{
    private readonly IDistributedCache _cache;
    private const string KeyPrefix = "AuthSession-";

    public RedisTicketStore(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(KeyPrefix + key);
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        // Create options
        var options = new DistributedCacheEntryOptions();

        if (ticket.Properties.ExpiresUtc.HasValue)
            options.AbsoluteExpiration = ticket.Properties.ExpiresUtc.Value;

        // Serialize
        var bytes = TicketSerializer.Default.Serialize(ticket);

        // Update token
        await _cache.SetAsync(KeyPrefix + key, bytes, options);
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        // Get token from db
        var bytes = await _cache.GetAsync(KeyPrefix + key);

        // Return if there is none
        if (bytes is null) return null;

        // Deserialize
        return TicketSerializer.Default.Deserialize(bytes);
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        // Generate session token
        string key = Guid.NewGuid().ToString();

        // Create options
        var options = new DistributedCacheEntryOptions();

        if (ticket.Properties.ExpiresUtc.HasValue)
            options.AbsoluteExpiration = ticket.Properties.ExpiresUtc.Value;
        else
            options.SlidingExpiration = TimeSpan.FromSeconds(600);

        // Serialize ticker
        var bytes = TicketSerializer.Default.Serialize(ticket);

        // Save to db
        await _cache.SetAsync(KeyPrefix + key, bytes, options);

        return key;
    }
}
