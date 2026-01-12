
using Redis.OM.Modeling;

namespace RedisShop.Models;

[Document(StorageType = StorageType.Json, Prefixes = new []{"CartItem"})]
public class CartItem
{
    [RedisIdField]
    public string? Id { get; set; }

    [Indexed]
    public string UserId { get; set; } = string.Empty;

    [Indexed]
    public string ProductId { get; set; } = string.Empty;
    
    public string ProductName { get; set; } = string.Empty;
    public float Price { get; set; }
    public int Quantity { get; set; }
}
