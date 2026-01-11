using Redis.OM;
using Redis.OM.Searching;
using RedisShop.Models;

namespace RedisShop.Services;

public class ProductService : IProductService
{
    private readonly RedisCollection<Product> _products;

    public ProductService(RedisConnectionProvider provider)
    {
        _products = (RedisCollection<Product>)provider.RedisCollection<Product>();
    }

    public async Task<IList<Product>> GetAllProductsAsync()
    {
        //todo add logging and exceptions
        return await _products.ToListAsync();
    }

    public async Task CreateProductAsync(Product product)
    {
        //todo add logging and exceptions
        await _products.InsertAsync(product);
    }
}
