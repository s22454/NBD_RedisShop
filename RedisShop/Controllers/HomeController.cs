using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RedisShop.Models;
using RedisShop.Services;

namespace RedisShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly IUserContext _userContext;

    public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService, IUserContext userContext)
    {
        _logger = logger;
        _productService = productService;
        _cartService = cartService;
        _userContext = userContext;
    }

    public async Task<IActionResult> Index()
    {
        // Get products in stock
        var products = await _productService.GetAllProductsAsync();

        var cartQuantities = new Dictionary<string, int>();

        // Check dose user is logged in
        if (_userContext.IsAuthenticated)
        {
            // Get user cart
            var cart = await _cartService.GetCartAsync(_userContext.UserId);
            cartQuantities = cart.ToDictionary(k => k.ProductId, v => v.Quantity);
        }

        var viewModelList = products.Select(p =>
        {
            // Check how much items are in user cart
            int inCart = (p.Id != null && cartQuantities.TryGetValue(p.Id, out int value)) ? value : 0;

            // Update visible stock
            return new ProductViewModel
            {
                Id = p.Id!,
                Name = p.Name,
                Price = p.Price,
                RealStock = p.Stock,
                InCartQuantity = inCart,
                AvailableStock = Math.Max(0, p.Stock - inCart)
            };
        }).ToList();

        return View(viewModelList);
    }

    [HttpPost]
    public async Task<IActionResult> AddTestProduct()
    {
        var test = new Product
        {
            Name = "test" + new Random().Next(1, 1000).ToString(),
            Price = new Random().Next(1, 1000),
            Stock = new Random().Next(1, 10)
        };

        await _productService.CreateProductAsync(test);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
