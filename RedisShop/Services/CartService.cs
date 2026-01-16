
using Redis.OM;
using Redis.OM.Searching;
using RedisShop.Models;

namespace RedisShop.Services;

public class CartService : ICartService
{
    private readonly RedisCollection<CartItem> _cart;

    public CartService(RedisConnectionProvider provider)
    {
        _cart = (RedisCollection<CartItem>)provider.RedisCollection<CartItem>();
    }

    public async Task AddToCartAsync(string userId, Product product, int quantity)
    {
        // Get user cart
        var userCart = await _cart.Where(c => c.UserId == userId).ToListAsync();

        // Check dose user have that item in cart
        var exisitingItem = await _cart.FirstOrDefaultAsync(c => c.ProductId == product.Id && c.UserId == userId);

        // Increase quantity if user have that product in cart
        if (exisitingItem is not null)
        {
            exisitingItem.Quantity += quantity;
            await _cart.UpdateAsync(exisitingItem);
        }

        // Add item to user cart if he doesn't have it
        else
        {
            CartItem newItem = new()
            {
                UserId = userId,
                ProductId = product.Id!,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity,
            };

            // Add cart item to db
            await _cart.InsertAsync(newItem);
        }
    }

    public async Task RemoveFromCartAsync(string userId, string productId, int quantity)
    {
        // Get user cart
        var cart = await _cart.Where(c => c.UserId == userId).ToListAsync();

        // Get item from cart
        var item = cart.FirstOrDefault(c => c.ProductId == productId);

        // Exist if there is a problem
        if (item is null || item.Quantity == 0) return;

        // Adjust item quantity in the cart
        item.Quantity -= quantity;

        // Update item quantity in cart or delete it if its 0
        if (item.Quantity == 0)
            await _cart.DeleteAsync(item);
        else
            await _cart.UpdateAsync(item);
    }


    public async Task ClearCartAsync(string userId)
    {
        await _cart
            .DeleteAsync(await _cart
                .Where(c => c.UserId == userId)
                .ToListAsync()
            );
    }

    public async Task<IList<CartItem>> GetCartAsync(string userId)
    {
        return await _cart
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }
}
