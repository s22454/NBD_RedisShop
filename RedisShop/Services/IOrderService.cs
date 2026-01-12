
namespace RedisShop.Services;

public interface IOrderService
{
    Task CheckoutAsync(string userId);
}
