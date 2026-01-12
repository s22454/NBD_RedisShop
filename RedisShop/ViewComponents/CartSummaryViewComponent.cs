
using Microsoft.AspNetCore.Mvc;
using RedisShop.Models.ViewModels;
using RedisShop.Services;

namespace RedisShop.ViewComponents;


//todo delete?
public class CartSummaryViewComponent : ViewComponent
{
    private readonly ICartService _cartService;
    private readonly IUserContext _userContext;

    public CartSummaryViewComponent(ICartService cartService, IUserContext userContext)
    {
        _cartService = cartService;
        _userContext = userContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // If user is not logged in cart is empty
        if (!_userContext.IsAuthenticated)
        {
            return View(new CartViewModel());
        }

        // Get user cart from db
        var userId = _userContext.UserId;
        var cart = await _cartService.GetCartAsync(userId);

        // Create view model
        CartViewModel model = new()
        {
            Items = cart,
            TotalPrice = cart.Sum(i => i.Price * i.Quantity)
        };

        // Send model to view
        return View(model);
    }
}
