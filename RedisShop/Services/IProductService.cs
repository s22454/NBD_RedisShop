using RedisShop.Models;

namespace RedisShop.Services;

public interface IProductService
{
    Task<IList<Product>> GetAllProductsAsync();
    Task CreateProductAsync(Product product);
}
