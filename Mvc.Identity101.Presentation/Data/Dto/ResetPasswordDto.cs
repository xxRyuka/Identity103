using System.ComponentModel.DataAnnotations;

namespace Mvc.Identity101.Data.Dto;

public class ResetPasswordDto
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string Token { get; set; }

    [Required(ErrorMessage ="Password is required")]
    public string Password { get; set; }
    
    [Compare(nameof(Password), ErrorMessage ="Passwords do not match")]
    [Required(ErrorMessage ="Confirm password is required")]
    public string ConfirmPassword { get; set; }
}