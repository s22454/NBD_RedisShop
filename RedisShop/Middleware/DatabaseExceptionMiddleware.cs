using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RedisShop.Middleware;

public class DatabaseExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DatabaseExceptionMiddleware> _logger;

    public DatabaseExceptionMiddleware(RequestDelegate next, ILogger<DatabaseExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Try to execute
            await _next(context);
        }
        catch (RedisConnectionException ex)
        {
            // Log db error
            _logger.LogError(ex, "REDIS database is offline!");

            // Redirect to offline page
            context.Response.Redirect("/Home/Offline");
        }
        catch (RedisTimeoutException ex)
        {
            _logger.LogError(ex, "REDIS TIMEOUT.");
            context.Response.Redirect("/Home/Offline");
        }
        catch (Exception e)
        {
            throw;
        }
    }
}
