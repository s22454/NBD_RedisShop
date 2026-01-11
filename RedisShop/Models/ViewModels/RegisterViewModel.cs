
using System.ComponentModel.DataAnnotations;

namespace RedisShop.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Login is required")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is requierd")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password needs to have at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Passwords don't match")]
    public string ConfirmedPassword { get; set; } = string.Empty;
}
