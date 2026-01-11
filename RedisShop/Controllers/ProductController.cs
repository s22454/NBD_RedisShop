
using Microsoft.AspNetCore.Mvc;
using RedisShop.Services;

namespace RedisShop.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }



}
