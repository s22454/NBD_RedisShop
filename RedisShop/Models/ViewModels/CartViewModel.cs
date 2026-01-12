
namespace RedisShop.Models.ViewModels;

public class CartViewModel
{
    public IList<CartItem> Items { get; set; } = [];
    public float TotalPrice { get; set; }
}
