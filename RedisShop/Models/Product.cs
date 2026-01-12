
using Redis.OM.Modeling;

namespace RedisShop.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new []{"Product"})]
    public class Product
    {
        [RedisIdField]
        public string? Id { get; set; }

        [Indexed]
        public string Name { get; set; } = string.Empty;

        [Indexed]
        public float Price { get; set; }

        [Indexed]
        public int Stock { get; set; }
    }
}
