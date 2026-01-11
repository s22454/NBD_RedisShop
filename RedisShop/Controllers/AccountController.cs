
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RedisShop.Models.ViewModels;
using RedisShop.Services;

namespace RedisShop.Controllers;

public class AccountController : Controller
{
    private static readonly string _className = "AccountController";
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // Check model state
        if (!ModelState.IsValid) return View(model);

        // Check user in db
        var user = await _userService.LoginAsync(model.Login, model.Password);

        // Return if login data are incorrect
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Incorrect login or password!");
            return View(model);
        }

        // Create claim
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, user.Login),
            new ("RedisUserId", user.Id!),
            new (ClaimTypes.Role, "User")
        };

        // Claim identity
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            RedirectUri = model.ReturnUrl
        };

        // Generate token and save it to db
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        // Redirection
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return LocalRedirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Validate model
        if (!ModelState.IsValid) return View(model);

        try
        {
            // Register user in db
            await _userService.RegisterAsync(model.Login, model.Password);

            // Redirect to login
            TempData["Success"] = "Registration successfull!";
            return RedirectToAction(nameof(Login));
        }
        catch (Exception e)
        {
            Utils.Logger.Log(_className, LogType.ERROR, $"Exception while registering user: {e.Message}");
            ModelState.AddModelError(string.Empty, e.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Logout()
    {
        // Delete user token
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirect to home page
        return RedirectToAction("Index", "Home");
    }
}
