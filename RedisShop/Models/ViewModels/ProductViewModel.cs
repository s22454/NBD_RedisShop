namespace RedisShop.Models;

public class ProductViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public float Price { get; set; }

    public int RealStock { get; set; }

    public int AvailableStock { get; set; }
    public int InCartQuantity { get; set; }
}
