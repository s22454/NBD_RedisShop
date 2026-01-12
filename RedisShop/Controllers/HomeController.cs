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

    public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
    {
        _logger = logger;
        _productService = productService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> AddTestProduct()
    {
        var test = new Product
        {
            Name = "test" + new Random().Next(1,1000).ToString(),
            Price = new Random().Next(1,1000),
            Stock = new Random().Next(1,10)
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
