
using RedisShop.Models;

namespace RedisShop.Services;

public interface ICartService
{
    Task AddToCartAsync(string userId, Product product, int quantity);
    Task RemoveFromCartAsync(string userId, string productId, int quantity);
    Task<IList<CartItem>> GetCartAsync(string userId);
    Task ClearCartAsync(string userId);
}
