using System.ComponentModel.DataAnnotations;

namespace Mvc.Identity101.Data.Dto;

public class ForgotPasswordDto
{
    [EmailAddress(ErrorMessage ="Email format is wrong")]
    [Required(ErrorMessage ="Email is required")]
    public string Email { get; set; }
}