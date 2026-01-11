
using Redis.OM.Modeling;

namespace RedisShop.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new []{"User"})]
    public class User
    {
        [RedisIdField]
        public string? Id { get; set; }

        [Indexed]
        public string Login { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
    }
}
