
using System.Text.Json;
using Redis.OM;
using Redis.OM.Searching;
using RedisShop.Models;

namespace RedisShop.Services;

public class DataSeeder
{
    private static readonly string _className = "DataSeeder";
    private readonly RedisCollection<Product> _products;

    public DataSeeder(RedisConnectionProvider provider)
    {
        _products = (RedisCollection<Product>)provider.RedisCollection<Product>();
    }

    public async Task SeedAsync()
    {
        // Check dose db contains products
        if (await _products.AnyAsync()) return;

        // Create file path
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "products.json");

        // Check dose file exist
        if (!File.Exists(filePath))
        {
            Utils.Logger.Log(_className, LogType.ERROR, $"Couln't load product data from {filePath}");
            return;
        }

        // Load and deserialize
        var jsonStr = await File.ReadAllBytesAsync(filePath);
        var products = JsonSerializer.Deserialize<List<Product>>(jsonStr);

        // Save to db
        if (products is not null && products.Any())
        {
            await _products.InsertAsync(products);
            Utils.Logger.Log(_className, LogType.SUCCESS, $"Successfully loaded {products.Count} products to DB.");
        }
    }
}
