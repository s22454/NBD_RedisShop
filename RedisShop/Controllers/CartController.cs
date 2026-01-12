
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedisShop.Models.ViewModels;
using RedisShop.Services;

namespace RedisShop.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly IUserContext _userContext;

    public CartController(ICartService cartService, IProductService productService, IOrderService orderService, IUserContext userContext)
    {
        _cartService = cartService;
        _productService = productService;
        _orderService = orderService;
        _userContext = userContext;
    }

    public async Task<IActionResult> Index()
    {
        // Get user id
        var userId = _userContext.UserId;

        // Get raw data from db
        var items = await _cartService.GetCartAsync(userId!);

        // Create view model
        CartViewModel model = new()
        {
            Items = items,
            TotalPrice = items.Sum(i => i.Price * i.Quantity)
        };

        // Send model to view
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Add(string productId)
    {
        // Get user id
        var userId = _userContext.UserId;

        // Get product from db
        var product = await _productService.GetByProductIdAsync(productId);

        // Check stock
        if (product is not null && product.Stock > 0)
        {
            await _cartService.AddToCartAsync(userId!, product, 1);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        // Get user id
        var userId = _userContext.UserId;

        try
        {
            // Ckeckout users cart
            await _orderService.CheckoutAsync(userId!);
            TempData["Success"] = "Order complete.";

            // Go back to main page
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            TempData["Error"] = e.Message;
            return RedirectToAction("Index");
        }
    }
}
