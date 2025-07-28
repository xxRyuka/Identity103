using System.ComponentModel.DataAnnotations;

namespace Mvc.Identity101.Data.Dto;

public class SignInDto
{
    [EmailAddress(ErrorMessage ="Email format is wrong")]
    [Required(ErrorMessage ="Email is required")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage ="Password is required")]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}