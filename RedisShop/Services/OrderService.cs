

using Redis.OM;
using Redis.OM.Searching;
using RedisShop.Models;

namespace RedisShop.Services;

public class OrderService : IOrderService
{
    private readonly ICartService _cartService;
    private readonly RedisCollection<Product> _products;

    public OrderService(ICartService cartService, RedisConnectionProvider provider)
    {
        _cartService = cartService;
        _products = (RedisCollection<Product>)provider.RedisCollection<Product>();
    }

    public async Task CheckoutAsync(string userId)
    {
        // Get users cart
        var cartItems = await _cartService.GetCartAsync(userId);

        // Check dose cart contains items
        //todo log and exceptions
        if (!cartItems.Any()) throw new Exception("Cart is empty");

        // Update stock based on users cart
        cartItems.ToList().ForEach(async i =>
        {
            // Get product from db
            var product = await _products.FindByIdAsync(i.ProductId);

            // Throw if product doesn't exist anymore
            if (product is null)
                throw new Exception($"Product {i.ProductName} is no longer available!");

            // Throw if product quantity is now lower then in cart
            if (product.Stock < i.Quantity)
                throw new Exception($"Product {i.ProductName}: not enough in stock anymore!");

            // Adjust product quantity in stock
            product.Stock -= i.Quantity;
            await _products.UpdateAsync(product);
        });

        // Clear user cart
        await _cartService.ClearCartAsync(userId);
    }
}
