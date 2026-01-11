
using System.ComponentModel.DataAnnotations;

namespace RedisShop.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Login is required")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is requierd")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
